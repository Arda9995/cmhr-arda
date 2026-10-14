using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class AppSettingsCachingService : BaseService, IAppSettingsCachingService
{
    public AppSettingsCachingService(IHttpContextAccessor httpContext, QuarkDataContext context,
        IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(
        httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult> RefreshAll()
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
}