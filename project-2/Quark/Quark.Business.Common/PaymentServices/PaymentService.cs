using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Model.V2;
using Iyzipay.Model.V2.Subscription;
using Iyzipay.Request.V2.Subscription;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.PaymentServices.Models.Requests;
using Quark.Business.Common.PaymentServices.Models.Responses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;
using Currency = Iyzipay.Model.Currency;

namespace Quark.Business.Common.PaymentServices;

public class PaymentService : BaseService, IPaymentService
{
    private readonly Options _options;

    public PaymentService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _options = new Options
        {
            ApiKey = _appSettingsKeys.IyzipayApiKey,
            BaseUrl = _appSettingsKeys.IyzicoBaseUrl,
            SecretKey = _appSettingsKeys.IyzicoSecretKey
        };
    }
    
    public ServiceResult<CreateProductResponse> CreateProduct(CreateCumhuriyetProductRequest request)
    {
        var result = new ServiceResult<CreateProductResponse>();
        
        CreateProductRequest productRequest = new CreateProductRequest
        {
            Description = request.Description,
            Locale = Locale.TR.ToString(),
            Name = request.Name,
            ConversationId = request.ConversationId ?? Guid.NewGuid().ToString()
        };
        
        var product = Product.Create(productRequest, _options);
        
        if (product.Status == Status.SUCCESS.ToString())
        {
            result.Data = new CreateProductResponse
            {
                Description = product.Data.Description,
                Name = product.Data.Name,
                ConversationId = productRequest.ConversationId,
                ReferenceCode = product.Data.ReferenceCode
            };
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(product.ErrorMessage);
        }

        return result;
    }

    public async Task<ServiceResult<CreateCumhuriyetPlanResponse>> CreatePaymentPlan(CreateCumhuriyetPlanRequest request)
    {
        var result = new ServiceResult<CreateCumhuriyetPlanResponse>();
        
        var isPopular = await _context.Plans.Where(x => x.IsPopular).AnyAsync();

        if (isPopular && request.IsPopular)
        {
            result.SetError("Sadece bir tane popüler plan oluşturabilirsiniz.");
            return result;
        }

        CreatePlanRequest planRequest = new CreatePlanRequest()
        {
            Locale = Locale.TR.ToString(),
            Name = request.Name,
            ConversationId = request.ConversationId ?? Guid.NewGuid().ToString(),
            Price = request.Price,
            CurrencyCode = request.Currency ?? Currency.TRY.ToString(),
            PaymentInterval = request.PaymentInterval ?? PaymentInterval.MONTHLY.ToString(),
            RecurrenceCount = request.RecurrenceCount,
            PaymentIntervalCount = request.PaymentIntervalCount,
            PlanPaymentType = request.PlanPaymentType ?? PlanPaymentType.RECURRING.ToString(),
            ProductReferenceCode = request.ProductReferenceCode,
        };
        
        var plan = Plan.Create(planRequest, _options);
        
        if (plan.Status == Status.SUCCESS.ToString())
        {
            result.Data = new CreateCumhuriyetPlanResponse
            {
                PlanName = plan.Data.Name,
                CurrencyCode = plan.Data.CurrencyCode,
                PaymentInterval = plan.Data.PaymentInterval,
                PaymentIntervalCount = plan.Data.PaymentIntervalCount,
                PlanPaymentType = plan.Data.PlanPaymentType,
                ReferenceCode = plan.Data.ReferenceCode,
                ConversationID = planRequest.ConversationId,
            };
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(plan.ErrorMessage);
        }
        
        return result;
    }

    public ServiceResult<DeleteCumhuriyetPlanResponse> DeletePaymentPlan(DeleteCumhuriyetPlanRequest request)
    {
        var result = new ServiceResult<DeleteCumhuriyetPlanResponse>();
        
        DeletePlanRequest deletePlanRequest = new DeletePlanRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = Guid.NewGuid().ToString(),
            PricingPlanReferenceCode = request.PricingPlanReferenceCode,
        };
        
        var deletePlan = Plan.Delete(deletePlanRequest, _options);
        
        if (deletePlan.Status == Status.SUCCESS.ToString())
        {
            result.Data = new DeleteCumhuriyetPlanResponse
            {
                ConversationID = deletePlanRequest.ConversationId
            };
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(deletePlan.ErrorMessage);
        }
        
        return result;
    }

    public async Task<ServiceResult<CreateCumhuriyetSubscriptionResponse>> CreateSubscription(CreateCumhuriyetSubscriptionRequest request)
    {
        var result = new ServiceResult<CreateCumhuriyetSubscriptionResponse>();

        var member = await (from m in _context.Members
            join c in _context.Cities on m.CityID equals c.ID
            join co in _context.Countries on c.CountryID equals co.ID
            where m.ID == CurrentUserID
            select new
            {
                member = m,
                city = c,
                country = co
            }).FirstOrDefaultAsync();

        var lastSubscription = await _context.Subscriptions.Where(s => s.MemberID == CurrentUserID).OrderByDescending(s => s.CreatedDate).FirstOrDefaultAsync();

        if (lastSubscription != null && lastSubscription.SubscriptionStatus == "ACTIVE" && lastSubscription.SubscriptionStatus != "CANCELED")
        {
            result.IsSuccess = false;
            result.SetError("Mevcut Aboneliğiniz devam etmektedir. Yeni bir abonelik oluşturamazsınız.");
            return result;
        }
        
        var plan = await _context.Plans
            .Where(x => x.ID == request.PlanID)
            .FirstOrDefaultAsync();
        
        SubscriptionInitializeRequest subscriptionRequest = new SubscriptionInitializeRequest
        {
            Locale = Locale.TR.ToString(),
            Customer = new CheckoutFormCustomer
            {
                Email = member.member.Email,
                Name = member.member.Firstname,
                Surname = member.member.Lastname,
                BillingAddress = new Address
                {
                    City = member.city.Name,
                    Country = member.country.Name,
                    Description = member.member.Address,
                    ContactName = member.member.FullName,
                    ZipCode = "34000",
                },
                ShippingAddress = new Address
                {
                    City = member.city.Name,
                    Country = member.country.Name,
                    Description = member.member.Address,
                    ContactName = member.member.FullName,
                    ZipCode = "34000",
                },
                GsmNumber = member.member.TypeID == (int)MemberTypeEnum.Individual ? member.member.MobilePhone : member.member.CompanyPhone,
                IdentityNumber = member.member.IdentityNumber,
            },
            PaymentCard = new CardInfo
            {
                CardNumber = request.PaymentCardNumber,
                CardHolderName = request.PaymentCardHolderName,
                ExpireMonth = request.PaymentCardExpireMonth,
                ExpireYear = request.PaymentCardExpireYear,
                Cvc = request.PaymentCardCVC,
                RegisterConsumerCard = true
            },
            ConversationId = request.ConversationId ?? Guid.NewGuid().ToString(),
            PricingPlanReferenceCode = plan.PlanReferenceCode,
            SubscriptionInitialStatus = request.SubscriptionInitialStatus ??  SubscriptionStatus.ACTIVE.ToString(),
        };
        
        var subscription = Subscription.Initialize(subscriptionRequest, _options);
        
        if (subscription.Status == Status.SUCCESS.ToString())
        {
            result.Data = new CreateCumhuriyetSubscriptionResponse
            {
                SubscriptionReferenceCode = subscription.Data.ReferenceCode,
                SubscriptionParentReferenceCode = subscription.Data.ParentReferenceCode,
                SubscriptionStatus = subscription.Data.SubscriptionStatus,
                ConversationId = subscriptionRequest.ConversationId,
                SubscriptionStartDate = subscription.Data.StartDate,
                SubscriptionPlanReferenceCode = subscription.Data.PricingPlanReferenceCode,
                CustomerReferenceCode = subscription.Data.CustomerReferenceCode,
            };
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(subscription.ErrorMessage);
            return result;
        }
        
        var paymentLog = new Data.Models.Payment.PaymentLog
        {
            CreatedDate = DateTime.Now,
            PaymentProviderSystemTime = subscription.SystemTime,
            PaymentReferenceCode = subscription.Data.ReferenceCode,
            ErrorGroup = subscription.ErrorGroup,
            ErrorCode = subscription.ErrorCode,
            Message = subscription.ErrorMessage,
            StatusCode = subscription.StatusCode,
            ConversationID = subscription.ConversationId,
            Status = subscription.Status
        };
        
        await _context.PaymentLogs.AddAsync(paymentLog);
        await _context.SaveChangesAsync();

        return result;
    }

    public ServiceResult<CancelCumhuriyetSubscriptionResponse> CancelSubscription(CancelCumhuriyetSubscriptionRequest request)
    {
        var result = new ServiceResult<CancelCumhuriyetSubscriptionResponse>();
        
        CancelSubscriptionRequest cancelSubscriptionRequest = new CancelSubscriptionRequest
        {
            Locale = Locale.TR.ToString(),
            SubscriptionReferenceCode = request.SubscriptionReferenceCode,
            ConversationId = request.ConversationId ?? Guid.NewGuid().ToString(),
        };
        
        var cancelSubscription = Subscription.Cancel(cancelSubscriptionRequest, _options);
        
        if (cancelSubscription.Status == Status.SUCCESS.ToString())
        {
            result.Data = new CancelCumhuriyetSubscriptionResponse
            {
                ConversationId = cancelSubscription.ConversationId,
                SystemTime = cancelSubscription.SystemTime
            };
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(cancelSubscription.ErrorMessage);
        }

        return result;
    }

    public async Task<ServiceResult> RetrySubscriptionPayment(string referenceCode)
    {
        var result = new ServiceResult();
        
        RetrySubscriptionRequest retrySubscriptionRequest = new RetrySubscriptionRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = Guid.NewGuid().ToString(),
            SubscriptionOrderReferenceCode = referenceCode
        };
        
        var retrySubscription = Subscription.Retry(retrySubscriptionRequest, _options);
        
        if (retrySubscription.Status == Status.SUCCESS.ToString())
        {
            result.IsSuccess = true;
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(retrySubscription.ErrorMessage);
        }
        
        var paymentLog = new Data.Models.Payment.PaymentLog
        {
            CreatedDate = DateTime.UtcNow,
            PaymentProviderSystemTime = retrySubscription.SystemTime,
            PaymentReferenceCode = referenceCode,
            ErrorGroup = retrySubscription.ErrorGroup,
            ErrorCode = retrySubscription.ErrorCode,
            Message = retrySubscription.ErrorMessage,
            StatusCode = retrySubscription.StatusCode,
            ConversationID = retrySubscription.ConversationId,
            Status = retrySubscription.Status
        };
        
        await _context.PaymentLogs.AddAsync(paymentLog);
        await _context.SaveChangesAsync();
        
        return result;
    }

    public ServiceResult UpgradeSubscription(UpgradeCumhuriyetSubscriptionRequest request)
    {
        var result = new ServiceResult();
        
        UpgradeSubscriptionRequest upgradeSubscriptionRequest = new UpgradeSubscriptionRequest
        {
            Locale = Locale.TR.ToString(),
            SubscriptionReferenceCode = request.SubscriptionReferenceCode,
            ResetRecurrenceCount = request.ResetRecurrenceCount,
            UseTrial = request.UseTrial,
            UpgradePeriod = request.UpgradePeriod,
            NewPricingPlanReferenceCode = request.NewPricingPlanReferenceCode,
            ConversationId = Guid.NewGuid().ToString()
        };
        
        var upgradeSubscription = Subscription.Upgrade(upgradeSubscriptionRequest, _options);
        
        if (upgradeSubscription.Status == Status.SUCCESS.ToString())
        {
            result.IsSuccess = true;
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(upgradeSubscription.ErrorMessage);
        }
        
        return result;
    }

    public ServiceResult<SubscriptionResource> SearchSubscriptionDetail(string referenceCode)
    {
        var result = new ServiceResult<SubscriptionResource>();
        
        SearchSubscriptionRequest searchSubscriptionRequest = new SearchSubscriptionRequest
        {
            Locale = Locale.TR.ToString(),
            SubscriptionReferenceCode = referenceCode,
            ConversationId = Guid.NewGuid().ToString(),
            Count = 1,
            Page = 1,
        };
        
        var searchSubscription = Subscription.Search(searchSubscriptionRequest, _options);
        
        if (searchSubscription.Status == Status.SUCCESS.ToString())
        {
            result.IsSuccess = true;
            result.Data = searchSubscription.Data.Items[0];
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(searchSubscription.ErrorMessage);
        }
        
        return result;
    }

    public ServiceResult<ResponsePagingData<SubscriptionResource>> GetSubscriptionDetail(string parentReferenceCode)
    {
        var result = new ServiceResult<ResponsePagingData<SubscriptionResource>>();
        
        SearchSubscriptionRequest searchSubscriptionRequest = new SearchSubscriptionRequest
        {
            Locale = Locale.TR.ToString(),
            ParentReferenceCode = parentReferenceCode,
            ConversationId = Guid.NewGuid().ToString(),
            Count = 99,
            Page = 1,
        };
        
        var searchSubscription = Subscription.Search(searchSubscriptionRequest, _options);
        
        if (searchSubscription.Status == Status.SUCCESS.ToString())
        {
            result.IsSuccess = true;
            result.Data = searchSubscription;
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(searchSubscription.ErrorMessage);
        }
        
        return result;
    }

    public ServiceResult ActivateSubscription(string referenceCode)
    {
        var result = new ServiceResult();
        
        ActivateSubscriptionRequest activateSubscriptionRequest = new ActivateSubscriptionRequest
        {
            Locale = Locale.TR.ToString(),
            SubscriptionReferenceCode = referenceCode,
            ConversationId = Guid.NewGuid().ToString(),
        };
        
        var activateSubscription = Subscription.Activate(activateSubscriptionRequest, _options);
        
        if (activateSubscription.Status == Status.SUCCESS.ToString())
        {
            result.IsSuccess = true;
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(activateSubscription.ErrorMessage);
        }
        
        return result;
    }
}