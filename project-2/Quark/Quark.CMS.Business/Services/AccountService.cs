using AvvaMobile.Core;
using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.SMSSender;
using AvvaMobile.Core.Utilities.FtpUpload;
using AvvaMobile.Core.Utilities.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.AccountRequests;
using Quark.CMS.Business.Models.Responses.AccountResponses;
using Quark.Data;
using Quark.Data.Models.LogModels;
using Quark.Resources;
using System.Security.Claims;
using Quark.Business.Caching.Interfaces;
using Quark.Data.Models.SystemLog;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class AccountService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    FileUpload fileUpload,
    AppSettingsKeys appSettingsKeys,
    IMailService mailService,
    ICacheManager cache,
    ISMSSender smsSender,
    ISystemLogCachingService _systemLogCachingService)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), IAccountService
{
    private readonly IMailService _mailService = mailService;

    public async Task<ServiceResult<SignInResponse>> SignInAsync(SignInRequest request)
    {
        var result = new ServiceResult<SignInResponse>();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.MobilePhone.Equals(request.Credential.ClearPhoneNumber()) || x.Email == request.Credential);

        if (user != null)
        {
            if (!user.IsEnabled)
            {
                var login1 = new SystemLog
                {
                    UserID = user.ID,
                    Credential = request.Credential,
                    IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                    IsSuccess = false,
                    Message = _localizer["UserIsNotEnabled"],
                    LogTypeID = (int)LogTypesEnum.LogIn
                };
                _context.SystemLogs.Add(login1);
                await _context.SaveChangesAsync();
                await _systemLogCachingService.RefreshAll();

                result.SetError(_localizer["UserIsNotEnabled"]);
                return result;
            }
            
            var signInToken = KeyGenerator.CreateAlphanumericCode(200);
            user.SignInToken = signInToken;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var login = new SystemLog
            {
                UserID = user.ID,
                Credential = request.Credential,
                IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                IsSuccess = true,
                Message = "User found, sign-in token generated.",
                LogTypeID = (int)LogTypesEnum.LogIn
            };
            _context.SystemLogs.Add(login);
            await _context.SaveChangesAsync();
            await _systemLogCachingService.RefreshAll();

            result.Data = new SignInResponse
            {
                SignInToken = signInToken,
                MobilePhone = user.MobilePhone
            };
        }
        else
        {
            var signInToken = KeyGenerator.CreateAlphanumericCode(200);

            result.Data = new SignInResponse
            {
                SignInToken = signInToken,
            };
        }

        return result;
    }
    
    public async Task<ServiceResult> SignInOTPAsync(SignInOTPRequest request)
    {
        var result = new ServiceResult();

        var user = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.SignInToken == request.SignInToken);

        if (user == null)
        {
            var login1 = new SystemLog
            {
                Credential = request.MobilePhone,
                IsSuccess = true,
                IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                Message = _localizer["LoginFailed"],
                LogTypeID = (int)LogTypesEnum.LogIn
            };
            _context.SystemLogs.Add(login1);
            await _context.SaveChangesAsync();
            await _systemLogCachingService.RefreshAll();

            result.SetError(_localizer["LoginFailed"]);
            return result;
        }

        if (user.Password != request.Password)
        {
            var login2 = new SystemLog
            {
                UserID = user.ID,
                Credential = request.MobilePhone,
                IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
                IsSuccess = true,
                Message = "User provided wrong password",
                LogTypeID = (int)LogTypesEnum.LogIn
            };
            _context.SystemLogs.Add(login2);
            await _context.SaveChangesAsync();
            await _systemLogCachingService.RefreshAll();

            result.SetError("Password is incorrect.");
            return result;
        }

        await _context.SaveChangesAsync();

        var userClaims = await _context.RoleClaims.Where(x => x.RoleID == user.RoleID).ToListAsync();

        var claims = new List<System.Security.Claims.Claim>
        {
            new (ClaimTypes.NameIdentifier, user.ID.ToString()),
            new (ClaimTypes.Name, user.FullName),
            new (ClaimTypes.Email, user.Email),
            new (ClaimTypes.MobilePhone, user.MobilePhone),
            new ("UserRoleName", user.Role.Name),
            new ("UserInitials", user.FullName.GetInitials())
        };

        claims.AddRange(userClaims.Select(x => new System.Security.Claims.Claim(ClaimTypes.Role, x.Key)));

        var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieNames.AuthCookieName));
        await _httpContext.HttpContext.SignInAsync(
            CookieNames.AuthCookieName,
            userPrincipal,
            new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(_appSettingsKeys.AuthCookie_Time),
                IsPersistent = true,
                AllowRefresh = true
            });

        var token = _httpContext.HttpContext.Response.GetTypedHeaders().SetCookie.FirstOrDefault(x => x.Name.Value == ".AspNetCore." + CookieNames.AuthCookieName);
        user.AuthToken = token.Value.Value;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        var login = new SystemLog
        {
            UserID = user.ID,
            Credential = request.MobilePhone,
            IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
            IsSuccess = true,
            Message = _localizer["LoginSuccess"],
            LogTypeID = (int)LogTypesEnum.LogIn
        };
        _context.SystemLogs.Add(login);
        await _context.SaveChangesAsync();
        await _systemLogCachingService.RefreshAll();

        return result;
    }
    
    public async Task<ServiceResult> SignOutAsync()
    {
        var result = new ServiceResult();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.ID == CurrentUserID);

        user.AuthToken = null;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        await _httpContext.HttpContext.SignOutAsync(CookieNames.AuthCookieName);
        
        var logout = new SystemLog
        {
            UserID = user.ID,
            Credential = user.Email,
            IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
            IsSuccess = true,
            Message = _localizer["LogoutSuccess"],
            LogTypeID = (int)LogTypesEnum.LogIn,
            ObjectID = null
        };
        _context.SystemLogs.Add(logout);
        await _context.SaveChangesAsync();
        await _systemLogCachingService.RefreshAll();

        return result;
    }
    
    public async Task<ServiceResult<UpdateProfileRequest>> GetProfile()
    {
        var result = new ServiceResult<UpdateProfileRequest>();
        var user = await (from us in _context.Users
                          join rl in _context.Roles on us.RoleID equals rl.ID
                          where us.ID == CurrentUserID
                          select new UpdateProfileRequest
                          {
                              ID = us.ID,
                              Firstname = us.Firstname,
                              Lastname = us.Lastname,
                              Email = us.Email,
                              MobilePhone = us.MobilePhone,
                              RoleName = rl.Name,
                              Password =  us.Password,
                              ProfilePhotoUrl = us.ProfilePhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_UsersImageFolder)
                          }).FirstOrDefaultAsync();

        if (user is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        result.Data = user;
        return result;
    }
    
    public async Task<ServiceResult> UpdateProfile(UpdateProfileRequest request)
    {
        var result = new ServiceResult();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.ID == CurrentUserID);

        if (user is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        user.Firstname = request.Firstname.Trim();
        user.Lastname = request.Lastname.Trim();
        user.Email = request.Email.Trim();
        user.MobilePhone = request.MobilePhone.ClearPhoneNumber();
        user.Password = request.Password;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        if (request.ProfilePhoto != null)
        {
            var uploadResult = fileUpload.Upload(request.ProfilePhoto, _appSettingsKeys.CDN_UsersImageFolder);
            if (uploadResult.IsSuccess)
            {
                user.ProfilePhotoUrl = uploadResult.FileName;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }
}