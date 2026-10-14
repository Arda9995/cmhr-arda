using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.AccountResponses;
using Quark.API.Business.Models.Responses.PaymentResponses;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;

namespace Quark.API.Business.Services
{
	public class SubscriptionService : BaseService, ISubscriptionService
	{
		public SubscriptionService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
		{
		}

		public async Task<ServiceResult<SubscriptionRequest>> GetSubscriptionInformations()
		{
			var result = new ServiceResult<SubscriptionRequest>
			{
				Data = new SubscriptionRequest
				{
					Plans = new List<PlanResponse>(),
				}
			};
			
			var plans = await _context.Plans
				.Where(x => x.IsEnabled && x.Price != 0)
				.Select(x => new PlanResponse
				{
					PlanID = x.ID,
					PaymentIntervalDisplayName = x.PaymentIntervalDisplayName,
					Price = x.Price
				})
				.ToListAsync();

			var currentUser = await _context.Members.FirstOrDefaultAsync(x => x.ID == CurrentUserID);

            if (currentUser.TypeID == null)
            {
                result.Data.IsEnableForPayment = true;
                result.SetError("Fatura tipi geçersiz.");
                return result;
            }

            if (currentUser.HasPremiumMembership)
			{
				result.SetError("Cumhuriyet Plus'a zaten sahip.");
				return result;
			}

			result.Data.Plans = plans;
			return result;
		}
	}
}
