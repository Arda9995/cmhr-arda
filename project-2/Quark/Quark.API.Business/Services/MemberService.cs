using AvvaMobile.Core.Business;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Handlers.JwtToken;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Requests.MemberRequests;
using Quark.API.Business.Models.Responses.MemberResponses;
using Quark.API.Business.Models.Responses.Notifications;
using Quark.Business.Common.BaseServices;
using Quark.Core.Extensions;
using Quark.Data;
using Quark.Resources;
using System.Security.Claims;
using AvvaMobile.Core.Caching;
using Iyzipay.Model.V2.Subscription;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Quark.Business.Common.PaymentServices;
using Quark.Business.Common.PaymentServices.Models.Requests;
using Quark.Data.Models;
using Quark.Data.Models.Payment;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.API.Business.Services
{
    public class MemberService : BaseService, IMemberService
    {
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;

        public MemberService(QuarkDataContext context, IHttpContextAccessor httpContext, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, IConfiguration configuration, ICacheManager cache, IPaymentService paymentService) :
            base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _configuration = configuration;
            _paymentService = paymentService;
        }

        public async Task<ServiceResult<Token>> CreateAsync(CreateMemberRequest request)
        {
            var result = new ServiceResult<Token>();

            var customer = await _context.Members.FindAsync(CurrentUserID);
            if (customer is null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            customer.Firstname = request.Firstname.Trim();
            customer.Lastname = request.Lastname.Trim();
            customer.IsEnabled = true;
            _context.Members.Update(customer);
            await _context.SaveChangesAsync();

            CustomTokenHandler tokenHandler = new(_configuration);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.ID.ToString()),
                new Claim(ClaimTypes.Name, customer.FullName),
                new Claim(ClaimTypes.MobilePhone, customer.MobilePhone),
            };
            var token = tokenHandler.CreateAccessToken(claims);

            result.Data = token;
            return result;
        }

        public async Task<ServiceResult<GetMemberResponse>> GetAsync(string mobileDevicePushID)
        {
            var result = new ServiceResult<GetMemberResponse>();

            var customer = await (from cus in _context.Members
                where cus.ID == CurrentUserID
                select new GetMemberResponse
                {
                    Firstname = cus.Firstname,
                    Lastname = cus.Lastname,
                    Email = cus.Email,
                    MobilePhone = cus.MobilePhone.ToPhoneNumber(),
                    ProfilePhotoUrl = cus.ProfilePhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_CustomersImageFolder),
                    IsEnabled = cus.IsEnabled,
                    GreetingMessage = DateTime.Now.GetGreetingMessage()
                }).FirstAsync();
            if (mobileDevicePushID != null)
            {
                var currentCustomer = await _context.Members.FindAsync(CurrentUserID);
                currentCustomer.MobileDevicePushID = mobileDevicePushID;
                _context.Members.Update(currentCustomer);
                await _context.SaveChangesAsync();
            }

            result.Data = customer;
            return result;
        }

        public async Task<ServiceResult<Token>> UpdateAsync(UpdateMemberRequest request)
        {
            var result = new ServiceResult<Token>();

            var customer = await _context.Members.FindAsync(CurrentUserID);
            if (customer == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            customer.Firstname = request.Firstname.Trim();
            customer.Lastname = request.Lastname.Trim();
            customer.Email = request.Email.Trim();

            _context.Members.Update(customer);
            await _context.SaveChangesAsync();

            CustomTokenHandler tokenHandler = new(_configuration);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.ID.ToString()),
                new Claim(ClaimTypes.Name, customer.FullName),
                new Claim(ClaimTypes.MobilePhone, customer.MobilePhone)
            };
            var token = tokenHandler.CreateAccessToken(claims);

            result.Data = token;
            return result;
        }

        public async Task<ServiceResult<List<ListNotificationsResponse>>> ListNotificationsAsync()
        {
            var result = new ServiceResult<List<ListNotificationsResponse>>();

            var notifications = await (from n in _context.Notifications
                let sender = _context.Members.FirstOrDefault(x => x.ID == n.NotificationSenderID)
                where n.MemberID == CurrentUserID
                select new ListNotificationsResponse
                {
                    ID = n.ID,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    ImageUrl = n.ImageUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_NotificationImagesFolder),
                    NotificationSenderID = sender == null ? null : sender.ID,
                    NotificationSenderName = sender == null ? null : sender.FullName,
                    NotificationSenderProfilePhoto = sender == null ? null : sender.ProfilePhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_CustomersImageFolder),
                    ElapsedTime = n.CreatedDate.CalculateElapsedTime()
                }).ToListAsync();
            result.Data = notifications;
            return result;
        }

        public async Task<ServiceResult> DeleteAsync()
        {
            var result = new ServiceResult();

            var customer = await _context.Members.FindAsync(CurrentUserID);
            if (customer == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            customer.IsDeleted = true;
            _context.Members.Update(customer);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<ServiceResult> CreateSubscriptionAsync(CreateCumhuriyetSubscriptionRequest createSubsRequest)
        {
            var result = new ServiceResult();
            
            var member = await _context.Members.FirstOrDefaultAsync(x => x.ID == CurrentUserID);
            if (member == null)
            {
                result.SetError(_localizer["UserNotFound"]);
                return result;
            }

            if (member.HasPremiumMembership)
            {
                result.SetError("Bu üyelik zaten aktif. Lütfen abonelik bilgilerinizi kontrol edin.");
                return result;
            }
            
            var subscriptionResult = _paymentService.CreateSubscription(createSubsRequest).Result;
            if (!subscriptionResult.IsSuccess)
            {
                result.SetError(subscriptionResult.Message ?? _localizer["SubscriptionNotCreated"]);
                return result;
            }
            
            var plan = await _context.Plans.FirstOrDefaultAsync(x => x.ID == createSubsRequest.PlanID);
            
            member.IyzicoCustomerReferenceCode = subscriptionResult.Data.CustomerReferenceCode;
            member.HasPremiumMembership = true;
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
            
            var subscription = new MemberSubscription()
            {
                MemberID = member.ID,
                SubscriptionReferenceCode = subscriptionResult.Data.SubscriptionReferenceCode,
                SubscriptionStatus = subscriptionResult.Data.SubscriptionStatus,
                CreatedDate = DateTime.Now,
                SubscriptionParentReferenceCode = subscriptionResult.Data.SubscriptionParentReferenceCode,
                PlanID = createSubsRequest.PlanID,
                ConversationId = subscriptionResult.Data.ConversationId,
                SubscriptionStartDate = DateTime.Now
			};
            
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
            
            var subscriptionDetail = _paymentService.SearchSubscriptionDetail(subscription.SubscriptionReferenceCode);

            if (subscriptionDetail == null)
            {
                result.SetError("Abonelik Oluşturuldu. Ancak sipariş kayıtları sisteme akıtılırken problemle karşılaşıldı");
                return result;
            }

            foreach (var subscriptionOrders in subscriptionDetail.Data.SubscriptionOrders)
            {
                var subscriptionOrder = new PaymentSubscriptionOrder()
                {
                    SubscriptionID = subscription.ID,
                    SubscriptionOrderReferenceCode = subscriptionOrders.ReferenceCode,
                    Price = subscriptionOrders.Price ?? string.Empty,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.Now.AddDays(plan.UsageDay),
                    OrderStatus = subscriptionOrders.OrderStatus,
                    InsertedDate = DateTime.Now 
                };
                    
                await _context.SubscriptionOrders.AddAsync(subscriptionOrder);
                await _context.SaveChangesAsync();
                    
                foreach (var orderAttemps in subscriptionOrders.OrderPaymentAttempts)
                {
                    var subscriptionOrderPaymentAttempt = new PaymentSubscriptionOrderPaymentAttempt()
                    {
                        SubscriptionOrderID = subscriptionOrder.ID,
                        ConversationId = orderAttemps.ConversationId,
                        PaymentID = orderAttemps.PaymentId,
                        PaymentStatus = orderAttemps.PaymentStatus,
                        ErrorCode = orderAttemps.ErrorCode,
                        ErrorMessage = orderAttemps.ErrorMessage,
                        CreatedDate = DateTime.Now,
                        InsertedDate = DateTime.Now
                    };
                    await _context.SubscriptionOrderPaymentAttempts.AddAsync(subscriptionOrderPaymentAttempt);
                }
            }
            
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<ServiceResult> RetrySubscriptionPayment(string referenceCode)
        {
            var result = new ServiceResult();
            var subscriptionResult = await _paymentService.RetrySubscriptionPayment(referenceCode);
            if (subscriptionResult.IsSuccess)
            {
                result.SetSuccess(_localizer["SubscriptionPaymentRetried"]);
            }
            else
            {
                result.IsSuccess = false;
                result.SetError(_localizer["SubscriptionPaymentRetryFailed"]);
            }

            return result;
        }

        public ServiceResult<SubscriptionResource> SearchSubscriptionDetail(string referenceCode)
        {
            var result = new ServiceResult<SubscriptionResource>();
            var subscriptionResult = _paymentService.SearchSubscriptionDetail(referenceCode);
            if (subscriptionResult.IsSuccess)
            {
                result.Data = subscriptionResult.Data;
                result.SetSuccess(_localizer["SubscriptionDetailsFetched"]);
            }
            else
            {
                result.IsSuccess = false;
                result.SetError(_localizer["SubscriptionDetailsFetchFailed"]);
            }

            return result;
        }
    }
}