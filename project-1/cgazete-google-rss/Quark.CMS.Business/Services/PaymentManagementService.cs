using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using Iyzipay.Model.V2.Subscription;
using Iyzipay.Request.V2.Subscription;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.PaymentServices;
using Quark.Business.Common.PaymentServices.Models.Requests;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.PaymentManagement;
using Quark.CMS.Business.Models.Responses.PaymentManagementResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.Payment;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;
using Plan = Quark.Data.Models.Payment.Plan;
using Product = Quark.Data.Models.Payment.Product;

namespace Quark.CMS.Business.Services;

public class PaymentManagementService : BaseService, IPaymentManagementService
{
    private readonly IPaymentService _paymentService;

    public PaymentManagementService(IHttpContextAccessor httpContext, QuarkDataContext context,
        IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache,
        IPaymentService paymentService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _paymentService = paymentService;
    }

    public async Task<ServiceResult> CreateProductAsync(CreateCumhuriyetProductRequest request)
    {
        var result = new ServiceResult();

        var addProduct = _paymentService.CreateProduct(request);

        if (addProduct.IsSuccess)
        {
            var product = new Product()
            {
                Description = addProduct.Data.Description,
                Name = addProduct.Data.Name,
                ConversationID = addProduct.Data.ConversationId,
                ReferenceCode = addProduct.Data.ReferenceCode,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = CurrentUserID,
                UpdatedDate = DateTime.UtcNow,
                UpdatedBy = CurrentUserID
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(addProduct.Message);
        }

        return result;
    }

    public async Task<ServiceResult> CreatePlanAsync(CreateCumhuriyetPlanRequest request)
    {
        var result = new ServiceResult();

        var createPlanRequest = await _paymentService.CreatePaymentPlan(request);

        if (createPlanRequest.IsSuccess)
        {
            var plan = new Plan()
            {
                Name = createPlanRequest.Data.PlanName,
                ConversationID = createPlanRequest.Data.ConversationID,
                PlanReferenceCode = createPlanRequest.Data.ReferenceCode,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = CurrentUserID,
                UpdatedDate = DateTime.UtcNow,
                UpdatedBy = CurrentUserID,
                Price = request.Price.ToDecimal(),
                CurrencyCode = request.Currency,
                PaymentInterval = request.PaymentInterval,
                PaymentIntervalDisplayName = request.PaymentIntervalDisplayName,
                PaymentIntervalCount = request.PaymentIntervalCount,
                RecurrenceCount = request.RecurrenceCount,
                PlanPaymentType = request.PlanPaymentType,
                ProductReferenceCode = request.ProductReferenceCode,
                IsPopular = request.IsPopular,
                UsageDay = CalculatePlanUsageDays(PaymentInterval.MONTHLY.ToString(), request.PaymentIntervalCount),
            };

            await _context.Plans.AddAsync(plan);
            await _context.SaveChangesAsync();
        }
        else
        {
            result.IsSuccess = false;
            result.SetError(createPlanRequest.Message);
        }

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetProductsSelectListItemAsync()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var products = await _context.Products.Select(p => new SelectListItem()
        {
            Text = p.Name,
            Value = p.ReferenceCode
        }).ToListAsync();

        result.IsSuccess = true;
        result.Data = products.AddSELECTOption();

        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponsePaymentPlansItem>>> GetAllPlansPagedAsync(
        BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<ListResponsePaymentPlansItem>>();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];
        string popularText = _localizer["Popular"];

        var query = (from b in _context.Plans.AsNoTracking()
            let activeSubscriptionCount = _context.Subscriptions.Count(x =>
                x.PlanID == b.ID && x.SubscriptionStatus == SubscriptionStatus.ACTIVE.ToString())
            select new
            {
                ID = b.ID,
                Name = b.Name,
                Price = b.Price.ToTL(),
                PaymentInterval = b.PaymentIntervalDisplayName,
                PlanReferenceCode = b.PlanReferenceCode,
                ProductReferenceCode = b.ProductReferenceCode,
                IsEnabled = b.IsEnabled,
                IsPopular = b.IsPopular,
                IsEnabledText = b.IsEnabled ? enabledText : disabledText,
                IsPopularText = b.IsPopular ? popularText : string.Empty,
                ActiveSubscriptionCount = activeSubscriptionCount,
            }).AsNoTracking().AsQueryable();

        result.RecordsTotal = query.Count();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x => x.Name.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponsePaymentPlansItem
        {
            ID = x.ID,
            Name = x.Name,
            Price = x.Price,
            PaymentInterval = x.PaymentInterval,
            PlanReferenceCode = x.PlanReferenceCode,
            ProductReferenceCode = x.ProductReferenceCode,
            IsEnabled = x.IsEnabled,
            IsPopular = x.IsPopular,
            IsEnabledText = x.IsEnabled ? enabledText : disabledText,
            IsPopularText = x.IsPopular ? popularText : string.Empty,
            ActiveSubscriptionCount = x.ActiveSubscriptionCount
        }).ToListAsync();

        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponsePaymentProductsItem>>> GetAllProductsPagedAsync(
        BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<ListResponsePaymentProductsItem>>();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        var query = (from b in _context.Products.AsNoTracking()
            select new
            {
                ID = b.ID,
                Name = b.Name,
                Description = b.Description,
                ProductReferenceCode = b.ReferenceCode,
                CreatedDate = b.CreatedDate.ToFormattedDate(),
                IsEnabled = b.IsEnabled,
                IsEnabledText = b.IsEnabled ? enabledText : disabledText
            }).AsNoTracking().AsQueryable();

        result.RecordsTotal = query.Count();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x => x.Name.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(b => new ListResponsePaymentProductsItem
        {
            ID = b.ID,
            Name = b.Name,
            Description = b.Description,
            ProductReferenceCode = b.ProductReferenceCode,
            CreatedDate = b.CreatedDate,
            IsEnabled = b.IsEnabled,
            IsEnabledText = b.IsEnabled ? enabledText : disabledText
        }).ToListAsync();

        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseSubscriptionItem>>> GetAllSubscriptionsPagedAsync(
        BaseDataTableRequest tableRequest)
    {
        var response = new BaseDataTableResponse<List<ListResponseSubscriptionItem>>();

        var query = (from b in _context.Subscriptions.AsNoTracking()
            join m in _context.Members on b.MemberID equals m.ID
            join pl in _context.Plans on b.PlanID equals pl.ID
            join p in _context.Products on pl.ProductReferenceCode equals p.ReferenceCode
            select new
            {
                ID = b.ID,
                MemberName = m.FullName,
                MemberID = m.ID,
                FirstName = m.Firstname,
                LastName = m.Lastname,
                SubscriptionReferenceCode = b.SubscriptionReferenceCode,
                Price = pl.Price.ToTL(),
                SubscriptionStatus = b.SubscriptionStatus,
                SubscriptionStatuClassName =
                    b.SubscriptionStatus == SubscriptionStatus.ACTIVE.ToString() ? "success" : 
                        (b.SubscriptionStatus == SubscriptionStatus.UPGRADED.ToString() ? "primary" : "danger"),
                SubscriptionStartDate = b.SubscriptionStartDate.ToFormattedDate(),
                SubscriptionEndDate = b.SubscriptionEndDate.ToFormattedDate(),
                SubscriptionPlanReferenceCode = pl.PlanReferenceCode
            }).AsNoTracking().AsQueryable();

        response.RecordsTotal = query.Count();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue) && response.RecordsTotal > 0)
        {
            query = query.Where(x => x.FirstName.Contains(tableRequest.SearchValue.ToLower())
                                     || x.LastName.ToLower().Contains(tableRequest.SearchValue.ToLower())
                                     || x.SubscriptionReferenceCode.ToLower().Contains(tableRequest.SearchValue.ToLower())
                                     || x.SubscriptionPlanReferenceCode.ToLower().Contains(tableRequest.SearchValue.ToLower()))
                .AsQueryable();
        }

        query = query.Skip(tableRequest.Skip).Take(tableRequest.PageSize).AsNoTracking().AsQueryable();

        response.Data = await query.Select(x => new ListResponseSubscriptionItem
        {
            ID = x.ID,
            MemberName = x.MemberName,
            MemberID = x.MemberID,
            SubscriptionReferenceCode = x.SubscriptionReferenceCode,
            Price = x.Price,
            SubscriptionStatus = x.SubscriptionStatus,
            SubscriptionStatuClassName = x.SubscriptionStatuClassName,
            SubscriptionStartDate = x.SubscriptionStartDate,
            SubscriptionEndDate = x.SubscriptionEndDate,
            SubscriptionPlanReferenceCode = x.SubscriptionPlanReferenceCode
        }).ToListAsync();

        response.IsSuccess = true;
        response.RecordsFiltered = response.Data.Count();

        return response;
    }

    public async Task<ServiceResult<SubscriptionsUpgradeRequest>> GetSubscriptionsUpgradeRequestAsync(int paymentPlanId)
    {
        var result = new ServiceResult<SubscriptionsUpgradeRequest>();

        var currentPlan = await _context.Plans.FirstOrDefaultAsync(x => x.ID == paymentPlanId);

        if (currentPlan == null)
        {
            result.IsSuccess = false;
            result.SetError("Ödeme Planı Bulunamadı.");
            return result;
        }

        var paymentPlans = await _context.Plans
            .Where(x => x.ProductReferenceCode == currentPlan.ProductReferenceCode
                        && x.ID != currentPlan.ID)
            .Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.ID.ToString()
            }).ToListAsync();

        var upgradeRequest = new SubscriptionsUpgradeRequest();
        upgradeRequest.CurrentPlanID = currentPlan.ID;
        upgradeRequest.CurrentPlanName = currentPlan.Name;
        upgradeRequest.PaymentPlans = paymentPlans.AddSELECTOption();

        result.IsSuccess = true;
        result.Data = upgradeRequest;
        return result;
    }

    public async Task<ServiceResult<SubscriptionsUpgradeRequest>> UpgradeSubscriptionAsync(
        SubscriptionsUpgradeRequest request)
    {
        var result = new ServiceResult<SubscriptionsUpgradeRequest>();

        var currentPlan = await _context.Plans.FirstOrDefaultAsync(x => x.ID == request.CurrentPlanID);
        var newPlan = await _context.Plans.FirstOrDefaultAsync(x => x.ID == request.SelectedPlanID);
        var subscritions = await _context.Subscriptions.Where(x => x.PlanID == currentPlan.ID
                                                                   && x.SubscriptionStatus ==
                                                                   SubscriptionStatus.ACTIVE.ToString()).ToListAsync();

        if (currentPlan == null || newPlan == null)
        {
            result.IsSuccess = false;
            result.SetError("Ödeme Planı Bulunamadı.");
            return result;
        }

        foreach (var subscrition in subscritions)
        {
            var upgradeRequest = new UpgradeCumhuriyetSubscriptionRequest()
            {
                SubscriptionReferenceCode = subscrition.SubscriptionReferenceCode,
                NewPricingPlanReferenceCode = newPlan.PlanReferenceCode,
            };

            var upgradeResponse = _paymentService.UpgradeSubscription(upgradeRequest);

            if (upgradeResponse.IsSuccess)
            {
                var newSubsDetail = _paymentService.GetSubscriptionDetail(subscrition.SubscriptionParentReferenceCode);
                if (newSubsDetail.IsSuccess)
                {
                    var activeSubsDetail = newSubsDetail.Data.Data.Items.FirstOrDefault(x =>
                        x.SubscriptionStatus == SubscriptionStatus.ACTIVE.ToString());
                    
                    var newSubscription = new MemberSubscription()
                    {
                        MemberID = subscrition.MemberID,
                        PlanID = newPlan.ID,
                        SubscriptionReferenceCode = activeSubsDetail.ReferenceCode,
                        SubscriptionParentReferenceCode = activeSubsDetail.ParentReferenceCode,
                        SubscriptionStatus = SubscriptionStatus.ACTIVE.ToString(),
                        SubscriptionStartDate = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                    };
                    
                    await _context.Subscriptions.AddAsync(newSubscription);
                    subscrition.SubscriptionStatus = SubscriptionStatus.UPGRADED.ToString();
                    subscrition.SubscriptionEndDate = subscrition.SubscriptionStartDate.AddDays(currentPlan.UsageDay);
                    
                    _context.Subscriptions.Update(subscrition);
                    await _context.SaveChangesAsync();

                    var subscriptionDetail =
                        _paymentService.SearchSubscriptionDetail(newSubscription.SubscriptionReferenceCode);

                    if (subscriptionDetail == null)
                    {
                        continue;
                    }

                    foreach (var subscriptionOrders in subscriptionDetail.Data.SubscriptionOrders)
                    {
                        var subscriptionOrder = new PaymentSubscriptionOrder()
                        {
                            SubscriptionID = newSubscription.ID,
                            SubscriptionOrderReferenceCode = subscriptionOrders.ReferenceCode,
                            Price = subscriptionOrders.Price ?? string.Empty,
                            StartPeriod = DateTime.Now,
                            EndPeriod = DateTime.Now.AddDays(newPlan.UsageDay),
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
                }
            }
        }

        await _context.SaveChangesAsync();
        result.IsSuccess = true;
        result.Data = request;
        return result;
    }

    public async Task<ServiceResult<UpdateCumhuriyetPlanRequest>> UpdatePlanAsync(int id)
    {
        var result = new ServiceResult<UpdateCumhuriyetPlanRequest>();
        var plan = await _context.Plans.FirstOrDefaultAsync(x => x.ID == id);
        if (plan == null)
        {
            result.IsSuccess = false;
            result.SetError("Plan Bulunamadı.");
            return result;
        }

        var updatePlanRequest = new UpdateCumhuriyetPlanRequest()
        {
            PlanID = plan.ID,
            Name = plan.Name,
            IsEnabled = plan.IsEnabled,
            PricingPlanReferenceCode = plan.PlanReferenceCode,
        };

        result.IsSuccess = true;
        result.Data = updatePlanRequest;
        return result;
    }

    public async Task<ServiceResult<UpdateCumhuriyetPlanRequest>> UpdatePlanAsync(UpdateCumhuriyetPlanRequest request)
    {
        var result = new ServiceResult<UpdateCumhuriyetPlanRequest>();
        var plan = await _context.Plans.FirstOrDefaultAsync(x => x.ID == request.PlanID);
        if (plan == null)
        {
            result.IsSuccess = false;
            result.SetError("Plan Bulunamadı.");
            return result;
        }

        plan.Name = request.Name;
        plan.IsEnabled = request.IsEnabled;
        _context.Plans.Update(plan);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        result.Data = request;
        return result;
    }

    public async Task<ServiceResult> ActivateSubscriptionAsync(string referenceCode)
    {
        var result = new ServiceResult();
        var subscription =
            await _context.Subscriptions.FirstOrDefaultAsync(x => x.SubscriptionReferenceCode == referenceCode);
        if (subscription == null)
        {
            result.IsSuccess = false;
            result.SetError("Abonelik Bulunamadı.");
            return result;
        }

        var subscriptionActivate = _paymentService.ActivateSubscription(referenceCode);

        if (!subscriptionActivate.IsSuccess)
        {
            result.IsSuccess = false;
            result.SetError(subscriptionActivate.Message);
            return result;
        }

        subscription.SubscriptionStatus = SubscriptionStatus.ACTIVE.ToString();
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();

        result.IsSuccess = true;
        return result;
    }

    public async Task<ServiceResult<SubscriptionDetailResponse>> GetSubscriptionDetail(int memberId)
    {
        var result = new ServiceResult<SubscriptionDetailResponse>();

        var member = await (from m in _context.Members

                            join c in _context.Countries on m.CountryID equals c.ID into memberCountries
                            from c in memberCountries.DefaultIfEmpty()

                            join d in _context.Districts on m.DistrictID equals d.ID into memberDistricts
                            from d in memberDistricts.DefaultIfEmpty()

                            join ci in _context.Cities on m.CityID equals ci.ID into memberCities
                            from ci in memberCities.DefaultIfEmpty()

                            where m.ID == memberId
                            select new
                            {
                                m,
                                c,
                                d,
                                ci
                            }).ToListAsync();


        if (!member.Any())
        {
            result.IsSuccess = false;
            result.SetError("Üye Bulunamadı.");
            return result;
        }
        
        var subscription = await (from s in _context.Subscriptions
                join p in _context.Plans on s.PlanID equals p.ID
                where s.MemberID == memberId
                    select new
                    {
                        s,p
                    }).ToListAsync();
        
        if (!subscription.Any())
        {
            result.IsSuccess = false;
            result.SetError("Abonelik Bulunamadı.");
            return result;
        }

        var subscriptionDetail = subscription.Select(x => new SubscriptionDetailResponseItem()
        {
            ID = x.s.ID,
            StartDate = x.s.SubscriptionStartDate.ToFormattedDate(),
            EndDate = x.s.SubscriptionEndDate.ToFormattedDate(),
            PaymentPlanName = x.p.Name,
            PaymentPlanPrice = x.p.Price.ToTL(),
            SubscriptionReferenceCode = x.s.SubscriptionReferenceCode,
            PaymentPlanReferenceCode = x.p.PlanReferenceCode,
            ProductReferenceCode = x.p.ProductReferenceCode,
            SubscriptionStatu = x.s.SubscriptionStatus,
        }).ToList();
        
        var subscriptionAddress = new SubscriptionAddress()
        {
            Address = member.FirstOrDefault().m.Address,
            City = member.FirstOrDefault().ci.Name,
            ZipCode = member.FirstOrDefault().m.ZipCode,
            Country = member.FirstOrDefault().c.Name,
            District = member.FirstOrDefault().d.Name,
            TaxOffice = member.FirstOrDefault().m.TaxOffice
        };
        
        var subscriptionDetailResponse = new SubscriptionDetailResponse()
        {
            BillingPersonName = member.FirstOrDefault().m.FullName,
            MemberID = memberId,
            SubscriptionAddress = subscriptionAddress,
            Email = member.FirstOrDefault().m.Email,
            PhoneNumber = member.FirstOrDefault().m.MobilePhone ?? member.FirstOrDefault().m.CompanyPhone,
            IdentityNumber = member.FirstOrDefault().m.IdentityNumber,
            MemberName = $"{member.FirstOrDefault().m.Firstname} {member.FirstOrDefault().m.Lastname}",
            IyzicoCustomerReferenceCode = subscription.FirstOrDefault().s.SubscriptionParentReferenceCode,
            SubscriptionDetailResponseItems = subscriptionDetail
        };
        
        result.IsSuccess = true;
        result.Data = subscriptionDetailResponse;
        return result;
    }

    private int CalculatePlanUsageDays(string paymentInterval, int planIntervalCount)
    {
        var planUsageDays = 0;

        if (paymentInterval == PaymentInterval.MONTHLY.ToString())
        {
            planUsageDays = 30 * planIntervalCount;
        }
        else if (paymentInterval == PaymentInterval.YEARLY.ToString())
        {
            planUsageDays = 365 * planIntervalCount;
        }
        else if (paymentInterval == PaymentInterval.WEEKLY.ToString())
        {
            planUsageDays = 7 * planIntervalCount;
        }
        else if (paymentInterval == PaymentInterval.DAILY.ToString())
        {
            planUsageDays = 1 * planIntervalCount;
        }

        return planUsageDays;
    }
}