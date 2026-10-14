using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Handlers.JwtToken;
using AvvaMobile.Core.SMSSender;
using AvvaMobile.Core.Utilities.Mail;
using DocumentFormat.OpenXml.Wordprocessing;
using Iyzipay.Model;
using Iyzipay.Request.V2.Subscription;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Requests.AccountRequests;
using Quark.API.Business.Models.Responses.AccountResponses;
using Quark.API.Business.Models.Responses.LoginResponses;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.PaymentServices;
using Quark.Business.Common.PaymentServices.Models.Requests;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.API.Business.Services
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly ISMSSender _smsSender;
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;
        private readonly IAccountCachingService _accountCachingService;
        private readonly ICommonCachingService _commonCachingService;

        public AccountService(IHttpContextAccessor httpContext, 
            QuarkDataContext context, IStringLocalizer<CommonResource> 
            localizer, AppSettingsKeys appSettingsKeys, 
            IConfiguration configuration, 
            ISMSSender smsSender, 
            ICacheManager cache, 
            IPaymentService paymentService, 
            IMailService mailService, 
            IAccountCachingService accountCachingService,
            ICommonCachingService commonCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _configuration = configuration;
            _smsSender = smsSender;
            _paymentService = paymentService;
            _mailService = mailService;
            _accountCachingService = accountCachingService;
            _commonCachingService = commonCachingService;
        }

        public async Task<ServiceResult> LoginAsync(LoginRequest request)
        {
            var result = new ServiceResult();

            //request.MobilePhone = request.MobilePhone.ClearPhoneNumber();

            //var member = _context.Members.FirstOrDefault(x => x.MobilePhone == request.MobilePhone && x.IsDeleted == false);
            //if (member == null)
            //{
            //    member = new Member
            //    {
            //        MobilePhone = request.MobilePhone,
            //        IsEnabled = false,
            //        IsDeleted = false,

            //    };
            //    _context.Members.Add(member);
            //    await _context.SaveChangesAsync();
            //}

            //if (!member.StaticSmsCode)
            //{
            //    var smsCode = new Random().Next(100000, 999999).ToString();

            //    var smsText = _localizer["SmsVerificationText"] + smsCode;
            //    var smsResult = await _smsSender.SendAsync(request.MobilePhone, smsText);

            //    member.SmsCodeExpireDate = DateTime.Now.AddMinutes(3);
            //    member.SmsCode = smsCode;
            //    _context.Members.Update(member);
            //    await _context.SaveChangesAsync();
            //}

            //result.SetSuccess(_localizer["SmsSendedForLogin"]);
            return result;
        }

        public async Task<ServiceResult<Token>> LoginEmailVerificationAsync(SmsVerificationRequest request)
        {
            var result = new ServiceResult<Token>();
            request.Email = request.Email;
            var member = _context.Members.FirstOrDefault(x => x.Email == request.Email && x.IsDeleted == false);

            if (member == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            if (member.EmailCode != request.EmailCode)
            {
                result.SetError(_localizer["VerificationCodeIsNotValid"]);
                return result;
            }

            if (DateTime.Now > member.EmailCodeExpireDate)
            {
                result.SetError(_localizer["VerificationCodeIsExpired"]);
                return result;
            }
            var RemainingDays = member.PremiumMembershipExpirationDate.HasValue ?
                    (int)(member.PremiumMembershipExpirationDate.Value - DateTime.Now).TotalDays : 0;
            CustomTokenHandler tokenHandler = new(_configuration);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, member.ID.ToString()),
                new Claim(ClaimTypes.Authentication, "true"),
                new Claim(ClaimTypes.Name, member.FullName),
                new Claim(ClaimTypes.Email, member.Email),
                new Claim(ClaimTypes.MobilePhone, member.MobilePhone),
                new Claim("HasPremiumMembership", RemainingDays > 0 ? "True" : member.HasPremiumMembership.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };
            var token = tokenHandler.CreateAccessToken(claims);

            member.RefreshToken = token.RefreshToken;
            member.RefreshTokenExpireDate = DateTime.Now.AddYears(1);
            member.LastLoginDate = DateTime.Now;
            _context.Members.Update(member);
            await _context.SaveChangesAsync();

            result.Data = token;
            return result;
        }

        public ServiceResult<Token> GetAnonymousToken(GetAnonymousTokenRequest request)
        {
            var result = new ServiceResult<Token>();

            var guid = Guid.NewGuid().ToString();

            CustomTokenHandler tokenHandler = new(_configuration);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, guid),
                new Claim(ClaimTypes.Authentication, "false"),
                new Claim("HasPremiumMembership", "false"),
                new Claim(JwtRegisteredClaimNames.Jti, guid)
            };
            var token = tokenHandler.CreateAccessToken(claims);

            result.Data = token;
            return result;
        }

        public async Task<ServiceResult<EmailLoginResponse>> EmailLoginAsync(EmailLoginRequest request)
        {
            var result = new ServiceResult<EmailLoginResponse>();

            var member = await _context.Members
                .FirstOrDefaultAsync(x => x.Email == request.Email && !x.IsDeleted);

            if (member == null)
            {
                result.SetError(_localizer["InvalidCredentials"]);
                return result;
            }

            if (!VerifyPassword(request.Password, member.Password))
            {
                result.SetError(_localizer["InvalidCredentials"]);
                return result;
            }

            if (member.HasPremiumMembership)
            {
                _httpContext.HttpContext.Response.Cookies.Append("_uana", "700b2301-ba6c-4d65-a3be-99e380dc0ffa", new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1),
                });
            }
            else
            {
                _httpContext.HttpContext.Response.Cookies.Append("_uana", "176af9ff-b327-4116-ad71-380b61833d80", new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1),
                });
            }
            var RemainingDays = member.PremiumMembershipExpirationDate.HasValue ?
                    (int)(member.PremiumMembershipExpirationDate.Value - DateTime.Now).TotalDays : 0;
            CustomTokenHandler tokenHandler = new(_configuration);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, member.ID.ToString()),
                new Claim(ClaimTypes.Authentication, "true"),
                new Claim(ClaimTypes.Name, member.FullName),
                new Claim(ClaimTypes.Email, member.Email),
                new Claim("HasPremiumMembership", RemainingDays > 0 ? "True" : member.HasPremiumMembership.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = tokenHandler.CreateAccessToken(claims);

            member.RefreshToken = token.RefreshToken;
            member.RefreshTokenExpireDate = request.RememberMe ?
                DateTime.Now.AddYears(1) : DateTime.Now.AddDays(30);
            member.LastLoginDate = DateTime.Now;

            await _context.SaveChangesAsync();

            var response = new EmailLoginResponse
            {
                IsNewUser = member.LastLoginDate == null,
                Token = token,
                HasPremiumMembership = member.HasPremiumMembership,
                PremiumMembershipExpirationDate = member.PremiumMembershipExpirationDate,
                RemainingDays = member.HasPremiumMembership && member.PremiumMembershipExpirationDate.HasValue ?
                    (int)(member.PremiumMembershipExpirationDate.Value - DateTime.Now).TotalDays : 0,
                MemberID = member.ID,
                FullName = member.FullName,
                Email = member.Email
            };

            _httpContext.HttpContext.Response.Cookies.Append("Authorization",
                $"Bearer {token.AccessToken}",
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(request.RememberMe ? 30 : 1)
                });

            result.Data = response;
            return result;
        }

        public async Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var result = new ServiceResult();

            var member = await _context.Members
                .FirstOrDefaultAsync(x => x.Email == request.Email && !x.IsDeleted);

            if (member == null)
            {
                result.SetSuccess("Kullanıcı Bulunamadı");
                return result;
            }

            var token = Guid.NewGuid().ToString();
            var newRandomPassword = new Random().Next(1000000, 9999999).ToString();


            member.Password = HashPassword(newRandomPassword);
            member.RefreshToken = token;
            member.RefreshTokenExpireDate = DateTime.Now.AddHours(24);


            await _context.SaveChangesAsync();


            var parameters = new Dictionary<string, string>
            {
                { "{{fullname}}", member.FullName },
                {"{{password}}", newRandomPassword}
            };
            var sendMail = await _mailService.Send(member.Email, "Şifre Sıfırlama", "ForgotPassword", parameters);

            result.SetSuccess(_localizer["PasswordResetEmailSent"]);
            return result;
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var result = new ServiceResult();

            var member = await _context.Members
                .FirstOrDefaultAsync(x => x.Email == request.Email && !x.IsDeleted);

            if (member == null)
            {
                result.SetError(_localizer["InvalidResetToken"]);
                return result;
            }

            if (member.RefreshToken != request.Token ||
                !member.RefreshTokenExpireDate.HasValue ||
                DateTime.Now > member.RefreshTokenExpireDate)
            {
                result.SetError(_localizer["InvalidResetToken"]);
                return result;
            }

            member.Password = HashPassword(request.NewPassword);

            member.RefreshToken = null;
            member.RefreshTokenExpireDate = null;

            await _context.SaveChangesAsync();

            result.SetSuccess(_localizer["PasswordResetSuccessful"]);
            return result;
        }

        public async Task<ServiceResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var result = new ServiceResult();

            var memberId = GetCurrentUserID();

            if (!memberId.HasValue)
            {
                result.SetError(_localizer["Unauthorized"]);
                return result;
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(x => x.ID == memberId.Value && !x.IsDeleted);

            if (member == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            if (!VerifyPassword(request.OldPassword, member.Password))
            {
                result.SetError(_localizer["OldPasswordIncorrect"]);
                return result;
            }

            member.Password = HashPassword(request.NewPassword);

            await _context.SaveChangesAsync();

            result.SetSuccess(_localizer["PasswordChangedSuccessfully"]);
            return result;
        }

        public async Task<ServiceResult<AccountMainPageResponse>> GetAccountInformationAsync()
        {
            var result = new ServiceResult<AccountMainPageResponse>
            {
                Data = new AccountMainPageResponse
                {
                    Cities = new List<SelectListItem>(),
                    District = new List<SelectListItem>(),
                }
            };

            var memberId = GetCurrentUserID();

            if (!memberId.HasValue)
            {
                result.SetError(_localizer["Unauthorized"]);
                return result;
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(x => x.ID == memberId.Value && !x.IsDeleted);

            if (member == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            var cities = await _context.Cities.OrderBy(x => x.Order).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.ID.ToString(),
            }).ToListAsync();

            var response = new AccountMainPageResponse
            {
                FullName = member.FullName,
                FirstName = member.Firstname,
                LastName = member.Lastname,
                Address = member.Address,
                PhoneNumber = member.MobilePhone,
                TCIdentityNumber = member.IdentityNumber,
                IsForeignCitizen = member.IsNonTurkish,
                BirthDay = member.BirthDate?.Day.ToString(),
                BirthMonth = member.BirthDate?.Month.ToString(),
                BirthYear = member.BirthDate?.Year.ToString(),
                TypeID = member.TypeID ?? 1,
                CompanyName = member.CompanyName,
                Email = member.Email,
                CompanyPhoneNumber = member.CompanyPhone,
                TaxOffice = member.TaxOffice,
                TaxNumber = member.CompanyIdentityNumber,
                CompanyAddress = member.CompanyAddress,
                CityID = member.CityID ?? 0,
                CountryID = member.CountryID ?? 0,
                DistrictID = member.DistrictID ?? 0,
                CompanyCityID = member.CompanyCityID ?? 0,
                CompanyCountryID = member.CompanyCountryID ?? 0,
                CompanyDistrictID = member.CompanyDistrictID ?? 0,
                BirthDate = member.BirthDate,
                Gender = member.Gender,
                HasPremiumMembership = member.HasPremiumMembership,
                MarketingConsent = member.MarketingConsent,
                PremiumMembershipExpirationDate = member.PremiumMembershipExpirationDate,
                RemainingDays = member.PremiumMembershipExpirationDate.HasValue ?
            (int)(member.PremiumMembershipExpirationDate.Value - DateTime.Now).TotalDays : 0,
                Cities = cities,
                ZipCode = member.ZipCode,
                CompanyZipCode = member.CompanyZipCode,
                BillingFullName = member.BillingFullName,
            };

            var plans = await _context.Plans.Where(x => x.IsEnabled && x.Price != 0).Select(x => new PlanResponse
            {
                PlanID = x.ID,
                PlanName = x.Name,
                Price = x.Price,
                PaymentIntervalDisplayName = x.PaymentIntervalDisplayName,
                IsPopular = x.IsPopular,
            }).ToListAsync();

            if (member.HasPremiumMembership)
            {
                var memberShip = await (from sb in _context.Subscriptions
                                        join so in _context.SubscriptionOrders on sb.ID equals so.SubscriptionID
                                        join p in _context.Plans on sb.PlanID equals p.ID
                                        join m in _context.Members on sb.MemberID equals m.ID
                                        where sb.MemberID == member.ID && sb.SubscriptionStatus == SubscriptionStatus.ACTIVE.ToString()
                                        select new PlanResponse
                                        {
                                            PlanName = p.Name,
                                            PaymentIntervalDisplayName = p.PaymentIntervalDisplayName,
                                            PlanID = p.ID,
                                            Price = p.Price,
                                            EndPeriod = so.EndPeriod,
                                            StartPeriod = so.StartPeriod,
                                        }).FirstOrDefaultAsync();
                if (memberShip != null)
                {
                    response.ActivePlan = memberShip;
                    response.PremiumMembershipExpirationDate = memberShip.EndPeriod;
                    response.RemainingDays = memberShip.EndPeriod.HasValue ? (int)(memberShip.EndPeriod.Value - DateTime.Now).TotalDays : 0;
                }
            }

            response.Plans = plans;
            result.Data = response;
            return result;
        }

        public async Task<ServiceResult> UpdateAccountInformationAsync(AccountMainPageResponse request)
        {
            var result = new ServiceResult();

            var memberId = GetCurrentUserID();

            if (!memberId.HasValue)
            {
                result.SetError(_localizer["Unauthorized"]);
                return result;
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(x => x.ID == memberId.Value && !x.IsDeleted);

            if (member == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            var nameParts = request.FullName?.Split(' ');
            if (nameParts != null && nameParts.Length > 0)
            {
                member.Firstname = nameParts[0];
                member.Lastname = string.Join(" ", nameParts.Skip(1));
            }

            member.TypeID = request.TypeID;
            member.MarketingConsent = request.MarketingConsent;

            if (member.TypeID == (int)MemberTypeEnum.Individual)
            {
                member.MobilePhone = request.PhoneNumber;
                member.Address = request.Address;
                member.IdentityNumber = request.TCIdentityNumber;
                member.CityID = request.CityID;
                member.DistrictID = request.DistrictID;
                member.ZipCode = request.ZipCode;
                member.BillingFullName = request.BillingFullName;
            }
            else if (member.TypeID == (int)MemberTypeEnum.Corporate)
            {
                member.CompanyIdentityNumber = request.CompanyIdentityNumber;
                member.CompanyAddress = request.CompanyAddress;
                member.CompanyPhone = request.CompanyPhoneNumber;
                member.CompanyName = request.CompanyName;
                member.CompanyCityID = request.CompanyCityID;
                member.CompanyDistrictID = request.CompanyDistrictID;
                member.CompanyZipCode = request.CompanyZipCode;
            }

            _context.Update(member);
            await _context.SaveChangesAsync();

            var cities = await _context.Cities.OrderBy(x => x.Order).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.ID.ToString(),
            }).ToListAsync();

            request.FirstName = member.Firstname;
            request.LastName = member.Lastname;
            request.Email = member.Email;
            request.BirthMonth = member.BirthDate?.Month.ToString();
            request.BirthYear = member.BirthDate?.Year.ToString();
            request.BirthDay = member.BirthDate?.Date.ToString();
            request.Gender = member.Gender;
            request.MarketingConsent = member.MarketingConsent;
            request.Cities = cities;

            result.SetSuccess(_localizer["AccountInformationUpdated"]);
            return result;
        }

        public async Task<ServiceResult> CancelPremiumMembershipRenewalAsync()
        {
            var result = new ServiceResult();

            var memberId = GetCurrentUserID();

            if (!memberId.HasValue)
            {
                result.SetError(_localizer["Unauthorized"]);
                return result;
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(x => x.ID == memberId.Value && !x.IsDeleted);

            if (member == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            var subscription = await _context.Subscriptions.FirstOrDefaultAsync(x => x.MemberID == member.ID
                && x.SubscriptionStatus == SubscriptionStatus.ACTIVE.ToString());

            var pricingPlan = await _context.Plans.FirstOrDefaultAsync(x => x.ID == subscription.PlanID);

            if (subscription == null)
            {
                result.SetError(_localizer["SubscriptionNotFound"]);
                return result;
            }

            var cancelRequest = new CancelCumhuriyetSubscriptionRequest
            {
                ConversationId = subscription.ConversationId,
                SubscriptionReferenceCode = subscription.SubscriptionReferenceCode,
            };

            var cancelResponse = _paymentService.CancelSubscription(cancelRequest);

            if (cancelResponse.IsSuccess)
            {
                subscription.SubscriptionStatus = SubscriptionStatus.CANCELED.ToString();
                subscription.SubscriptionEndDate = subscription.SubscriptionStartDate.AddDays(pricingPlan.UsageDay);
                _context.Subscriptions.Update(subscription);

                member.HasPremiumMembership = false;
                member.PremiumMembershipExpirationDate = subscription.SubscriptionEndDate;
                _context.Members.Update(member);

                await _context.SaveChangesAsync();
            }

            result.SetSuccess(_localizer["MembershipAutoRenewalCanceled"]);
            return result;
        }

        public async Task<ServiceResult> AccountInformations(RegistrationRequest request)
        {

            var result = new ServiceResult();

            if (GetCurrentUserID() == null)
            {
                result.SetError("Kayıt olunuz.");
                return result;
            }

            var currentMember = await _context.Members.FindAsync(CurrentUserID);

            if (currentMember == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            currentMember.TypeID = request.TypeID;
            currentMember.CompanyPhone = request.CompanyPhone;
            currentMember.IdentityNumber = request.IdentityNumber;
            currentMember.Gender = request.Gender;
            currentMember.Address = request.Address;
            currentMember.CompanyAddress = request.CompanyAddress;
            currentMember.CompanyCityID = (request.SelectedCompanyCityID > 0) ? request.SelectedCompanyCityID : null;
            currentMember.CompanyDistrictID = (request.SelectedCompanyDisctrictID > 0) ? request.SelectedCompanyDisctrictID : null;
            currentMember.CompanyCountryID = (request.SelectedCompanyCountryID > 0) ? request.SelectedCompanyCountryID : null;
            currentMember.CompanyName = request.CompanyName;
            currentMember.CompanyIdentityNumber = request.CompanyIdentityNumber;
            currentMember.BirthDate = new DateTime(request.BirthYear, request.BirthMonth, request.BirthDay);
            currentMember.MobilePhone = request.MobilePhone;
            currentMember.TaxOffice = request.TaxOffice;
            currentMember.CityID = (request.SelectedCityID > 0) ? request.SelectedCityID : null;
            currentMember.CountryID = (request.SelectedCountryID > 0) ? request.SelectedCountryID : null;
            currentMember.DistrictID = (request.SelectedDisctrictID > 0) ? request.SelectedDisctrictID : null;
            currentMember.IsNonTurkish = request.IsForeignCitizen;
            currentMember.MarketingConsent = false;
            currentMember.BillingFullName = request.FullName;

            var countries = await _context.Countries.Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name,
            }).ToListAsync();

            if (request.IsForeignCitizen)
            {
                currentMember.IdentityNumber = "11111111111";
                currentMember.CompanyIdentityNumber = "11111111111";
            }

            request.Countries = countries.AddSELECTOption();
            _context.Members.Update(currentMember);
            await _context.SaveChangesAsync();

            result.SetSuccess(_localizer["UpdateSuccess"]);
            return result;
        }

        public async Task<ServiceResult> LogoutAsync()
        {
            var result = new ServiceResult();

            var memberId = GetCurrentUserID();
            if (!memberId.HasValue)
            {
                result.SetError(_localizer["Unauthorized"]);
                return result;
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(x => x.ID == memberId.Value && !x.IsDeleted);

            if (member == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            member.RefreshToken = null;
            member.RefreshTokenExpireDate = null;

            await _context.SaveChangesAsync();

            _httpContext.HttpContext.Response.Cookies.Delete("Authorization");
            _httpContext.HttpContext.Response.Cookies.Delete("_uana");

            result.SetSuccess(_localizer["LogoutSuccessful"]);
            return result;
        }

        #region Helper Methods

        private string HashPassword(string password)
        {
            // Using SHA256 for password hashing (in a real application, use a proper password hashing library)
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
                return false;

            var newHash = HashPassword(password);
            return newHash == hashedPassword;
        }

        private int? GetCurrentUserID()
        {
            try
            {
                if (_httpContext?.HttpContext?.User == null)
                {
                    return null;
                }

                var claims = _httpContext.HttpContext.User.Claims.ToList();
                if (claims == null || !claims.Any())
                {
                    return null;
                }

                var nameIdentifier = _httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(nameIdentifier))
                {
                    return null;
                }

                if (!int.TryParse(nameIdentifier, out var memberId))
                {
                    return null;
                }

                return memberId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ServiceResult<RegistrationRequest>> AccountInformations()
        {
            var result = new ServiceResult<RegistrationRequest>
            {
                Data = new RegistrationRequest
                {
                    Countries = new List<SelectListItem>(),
                }
            };

            var countries = await _context.Countries.OrderBy(x => x.Order).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.ID.ToString(),
            }).ToListAsync();

            var userCreated = await _context.Members.FindAsync(CurrentUserID);

            if (userCreated != null && userCreated.TypeID != null)
            {
                result.SetError("Kullanıcı bilgileri zaten oluşturuldu");
                return result;
            }

            result.Data.Countries = countries.AddSELECTOption();
            return result;
        }

        public async Task<ServiceResult<RegistrationResponse>> RegisterAsync(RegistrationRequest request)
        {
            var result = new ServiceResult<RegistrationResponse>();

            if (GetCurrentUserID() != null)
            {
                result.SetError("Çıkış yapınız.");
                return result;
            }

            var emailExists = await _context.Members.AnyAsync(x => x.Email == request.Email && !x.IsDeleted);
            if (emailExists)
            {
                result.SetError(_localizer["EmailAlreadyExists"]);
                return result;
            }

            var member = new Member
            {
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                HasPremiumMembership = false,
                IsDeleted = false,
                IsEnabled = true,
                CreatedDate = DateTime.Now,
                RegistrationDate = DateTime.Now,
                Password = HashPassword(request.Password),
                Email = request.Email,
                EmailSmsCode = false,
                IsNonTurkish = false,
            };

            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
            await _accountCachingService.RefreshMembers();

            result.SetSuccess(_localizer["RegisterSuccess"]);

            result.Data = new RegistrationResponse
            {
                Email = member.Email,
                Fullname = member.Firstname + " " + member.Lastname,
                UserID = member.ID,
            };
            return result;
        }

        public async Task<ServiceResult> RegisterAsync()
        {
            var result = new ServiceResult();

            if (GetCurrentUserID() != null)
            {
                result.SetError("Çıkış yapınız.");
                return result;
            }

            return result;
        }

        public async Task<ServiceResult<GetMembershipIntroductionResponse>> MembershipIntroduction()
        {
            var result = new ServiceResult<GetMembershipIntroductionResponse>();


            var populerplanInformationsData = await _accountCachingService.GetPlans();
            var populerplanInformations = populerplanInformationsData.Data.OrderByDescending(x => x.ID).FirstOrDefault(x => x.IsEnabled);

            if (populerplanInformations == null)
            {
                result.SetError(_localizer["PlanNotFound"]);
                return result;
            }

            if (GetCurrentUserID() == null)
            {
                var responseNonMember = new GetMembershipIntroductionResponse
                {
                    Fullname = string.Empty,
                    PlanType = populerplanInformations.PaymentIntervalDisplayName,
                    PlanPrice = populerplanInformations.Price.ToString(),
                };

                result.Data = responseNonMember;
                return result;
            }

            var membersData = await _accountCachingService.GetMembers();
            var currentMember = membersData.Data.FirstOrDefault(x => x.ID == CurrentUserID && !x.IsDeleted);

            var response = new GetMembershipIntroductionResponse
            {
                Fullname = currentMember.FullName,
                PlanType = populerplanInformations.PaymentIntervalDisplayName,
                PlanPrice = populerplanInformations.Price.ToString(),
            };

            result.Data = response;
            return result;
        }
        #endregion
    }
}