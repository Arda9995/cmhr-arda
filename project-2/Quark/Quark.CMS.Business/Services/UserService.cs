using AvvaMobile.Core;
using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.FtpUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.UserRequests;
using Quark.CMS.Business.Models.Responses.UserResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AvvaMobile.Core.Caching;
using Quark.Business.Caching.Interfaces;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class UserService : BaseService, IUserService
{
    private readonly FileUpload _fileUpload;
    private readonly IUserCachingService _userCachingService;
    private readonly ICategoryCachingService _categoryCachingService;

    public UserService(QuarkDataContext context, IHttpContextAccessor httpContext, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, FileUpload fileUpload, IUserCachingService userCachingService, INewsCachingService newsCachingService, ICategoryCachingService categoryCachingService) : base(httpContext, context, localizer,
        appSettingsKeys, cache)
    {
        _fileUpload = fileUpload;
        _userCachingService = userCachingService;
        _categoryCachingService = categoryCachingService;
    }

    public ServiceResult<GetFiltersRequest> GetAllPagedFiltersAsync()
    {
        var result = new ServiceResult<GetFiltersRequest>();
        var filters = new GetFiltersRequest { IsEnabled = true };
        result.Data = filters;
        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, GetFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<ListResponseItem>>();

        result.RecordsTotal = _context.Users.Count();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        var query = (from us in _context.Users
            join ro in _context.Roles on us.RoleID equals ro.ID
            orderby us.Firstname, us.Lastname
            select new ListResponseItem
            {
                ID = us.ID,
                Firstname = us.Firstname,
                Lastname = us.Lastname,
                MobilePhone = us.MobilePhone,
                Email = us.Email,
                ProfilePhotoUrl = us.ProfilePhotoUrl,
                IsEnabled = us.IsEnabled,
                IsEnabledText = us.IsEnabled ? enabledText : disabledText,
                RoleName = ro.Name
            }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Firstname.ToLower().Contains(tableRequest.SearchValue.ToLower()) ||
                x.Lastname.ToLower().Contains(tableRequest.SearchValue.ToLower()) ||
                x.Email.ToLower().Contains(tableRequest.SearchValue.ToLower()) ||
                x.MobilePhone.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Firstname = x.Firstname,
            Lastname = x.Lastname,
            MobilePhone = x.MobilePhone.ToPhoneNumber(),
            Email = x.Email,
            ProfilePhotoUrl = x.ProfilePhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_UsersImageFolder),
            IsEnabled = x.IsEnabled,
            IsEnabledText = x.IsEnabledText,
            RoleName = x.RoleName
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<CreateRequest>> CreateAsync()
    {
        var result = new ServiceResult<CreateRequest>();
        var createUser = new CreateRequest();

        createUser.Roles = await (from rl in _context.Roles
            select new SelectListItem
            {
                Value = rl.ID.ToString(),
                Text = rl.Name
            }).ToListAsync();
        createUser.Roles.AddSELECTOption();

        var categoriesList = await _categoryCachingService.GetCategoriesAsSelectListItems();
        createUser.Categories = categoriesList.Data;
        
        result.Data = createUser;
        return result;
    }

    public async Task<ServiceResult<CreateRequest>> CreateAsync(CreateRequest request)
    {
        var result = new ServiceResult<CreateRequest> { Data = request };

        var existedUser = await _context.Users.Where(x => x.Email == request.Email.Trim() || x.MobilePhone == request.MobilePhone.ClearPhoneNumber()).FirstOrDefaultAsync();

        if (existedUser != null)
        {
            request.Roles = await (from rl in _context.Roles
                select new SelectListItem
                {
                    Value = rl.ID.ToString(),
                    Text = rl.Name
                }).ToListAsync();
            request.Roles.AddSELECTOption();

            result.SetError(_localizer["UserEmailOrMobilePhoneExist"]);
            return result;
        }

        if (request.ProfilePhoto != null)
        {
            var uploadResult = _fileUpload.Upload(request.ProfilePhoto, _appSettingsKeys.CDN_UsersImageFolder);
            if (uploadResult.IsSuccess)
            {
                request.ProfilePhotoUrl = uploadResult.FileName;
            }
        }

        var user = new User
        {
            Firstname = request.Firstname.Trim(),
            Lastname = request.Lastname.Trim(),
            Email = request.Email.Trim(),
            MobilePhone = request.MobilePhone.ClearPhoneNumber(),
            RoleID = request.SelectedRoleID,
            IsEnabled = true,
            ProfilePhotoUrl = request.ProfilePhotoUrl,
            Password = request.Password.Trim(),
            IsPasswordConstant = true
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        foreach (var categoryID in request.SelectedCategoryIds)
        {
            var userCategory = new UserCategory
            {
                UserID = user.ID,
                CategoryID = categoryID
            };

            await _context.UserCategories.AddAsync(userCategory);
        }
        
        await _context.SaveChangesAsync();
        await _userCachingService.RefreshAll();
            
        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult<UpdateRequest>> UpdateAsync(int id)
    {
        var result = new ServiceResult<UpdateRequest>();

        var user = await (from us in _context.Users
            where us.ID == id
            select new UpdateRequest
            {
                ID = us.ID,
                Firstname = us.Firstname,
                Lastname = us.Lastname,
                Email = us.Email,
                Password = us.Password,
                MobilePhone = us.MobilePhone.ToPhoneNumber(),
                ProfilePhotoUrl = us.ProfilePhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_UsersImageFolder),
                SelectedRoleID = us.RoleID,
                IsEnabled = us.IsEnabled,
            }).FirstOrDefaultAsync();
        if (user is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        user.Roles = await (from rl in _context.Roles
            select new SelectListItem
            {
                Value = rl.ID.ToString(),
                Text = rl.Name,
                Selected = user.SelectedRoleID == rl.ID
            }).ToListAsync();
        user.Roles.AddSELECTOption();

        var categoriesList = await _categoryCachingService.GetCategoriesAsSelectListItems();
        user.Categories = categoriesList.Data;
        
        user.SelectedCategoryIds = await _context.UserCategories.Where(x => x.UserID == id)
            .Select(x => x.CategoryID).ToListAsync();

        result.Data = user;
        return result;
    }

    public async Task<ServiceResult<UpdateRequest>> UpdateAsync(UpdateRequest request)
    {
        var result = new ServiceResult<UpdateRequest> { Data = request };

        var user = await _context.Users.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (user is null)
        {
            result.Data.Roles = await (from rl in _context.Roles
                select new SelectListItem
                {
                    Value = rl.ID.ToString(),
                    Text = rl.Name,
                }).ToListAsync();
            result.Data.Roles.AddSELECTOption();

            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        if (request.ProfilePhoto != null)
        {
            var uploadResult = _fileUpload.Upload(request.ProfilePhoto, _appSettingsKeys.CDN_UsersImageFolder);
            if (uploadResult.IsSuccess)
            {
                request.ProfilePhotoUrl = uploadResult.FileName;
            }
        }

        user.Firstname = request.Firstname;
        user.Lastname = request.Lastname;
        user.Email = request.Email;
        user.MobilePhone = request.MobilePhone.ClearPhoneNumber();
        user.RoleID = request.SelectedRoleID;
        user.IsEnabled = request.IsEnabled;
        user.ProfilePhotoUrl = request.ProfilePhotoUrl;
        user.Password = request.Password;

        var userCategories = await _context.UserCategories.Where(x => x.UserID == user.ID).ToListAsync();
        if (userCategories.Count != 0)
        {
            _context.RemoveRange(userCategories);
        }
        
        foreach (var categoryID in request.SelectedCategoryIds)
        {
            var userCategory = new UserCategory
            {
                UserID = user.ID,
                CategoryID = categoryID
            };

            await _context.UserCategories.AddAsync(userCategory);
        }
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        await _userCachingService.RefreshAll();
            
        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var result = new ServiceResult();

        var user = await _context.Users.FirstOrDefaultAsync(x => x.ID == id);
        if (user is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
        
        var userCategories = await _context.UserCategories.Where(x => x.UserID == user.ID).ToListAsync();
        _context.RemoveRange(userCategories);
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
            
        await _userCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }
}