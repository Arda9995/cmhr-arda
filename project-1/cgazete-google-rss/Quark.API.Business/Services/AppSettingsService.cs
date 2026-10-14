using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;

namespace Quark.API.Business.Services
{
    public class AppSettingsService : BaseService, IAppSettingsService
    {
        public AppSettingsService(QuarkDataContext context, IHttpContextAccessor httpContext, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
        }

        public async Task<ServiceResult> LoadAllSettingsToCache()
        {
            var result = new ServiceResult();

            var appSettings = await (from ap in _context.AppSettings
                                     orderby ap.Order
                                     select ap).ToListAsync();

            foreach (var item in appSettings)
            {
                await _cache.Set(item.Key, item.Value);
            }
            return result;
        }
        
        public ServiceResult<Dictionary<string, string>> AppVersions()
        {
            var result = new ServiceResult<Dictionary<string, string>>();
            result.Data = new Dictionary<string, string>();

            result.Data.Add("iPhone", _cache.Get("MobileApp_iPhoneVersion").Result);
            result.Data.Add("iPad", _cache.Get("MobileApp_iPadVersion").Result);
            result.Data.Add("Android", _cache.Get("MobileApp_GooglePlayVersion").Result);
            result.Data.Add("Huawei", _cache.Get("MobileApp_HuaweiVersion").Result);

            return result;
        }
    }
}