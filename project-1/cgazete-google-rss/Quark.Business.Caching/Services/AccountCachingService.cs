using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.Payment;
using Quark.Resources;

namespace Quark.Business.Caching.Services
{
    public class AccountCachingService : BaseService, IAccountCachingService
    {
        public AccountCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
        }

        public async Task<ServiceResult<List<Member>>> GetMembers()
        {
            var result = new ServiceResult<List<Member>>();

            result.Data = await _cache.Get<List<Member>>(RedisKeys.Members);
            if (result.Data == null)
            {
                await RefreshMembers();
                result.Data = await _cache.Get<List<Member>>(RedisKeys.Members);
            }

            return result;
        }

        public async Task<ServiceResult<List<Plan>>> GetPlans()
        {
            var result = new ServiceResult<List<Plan>>();

            result.Data = await _cache.Get<List<Plan>>(RedisKeys.Plans);
            if (result.Data == null)
            {
                await RefreshPlans();
                result.Data = await _cache.Get<List<Plan>>(RedisKeys.Plans);
            }

            return result;
        }

        public async Task<ServiceResult> RefreshMembers()
        {
            var result = new ServiceResult();
            var members = await _context.Members.ToListAsync();
            await _cache.Set(RedisKeys.Members, members);
            return result;
        }

        public async Task<ServiceResult> RefreshPlans()
        {
            var result = new ServiceResult();
            var plans = await _context.Plans.ToListAsync();
            await _cache.Set(RedisKeys.Plans, plans);
            return result;
        }
    }
}
