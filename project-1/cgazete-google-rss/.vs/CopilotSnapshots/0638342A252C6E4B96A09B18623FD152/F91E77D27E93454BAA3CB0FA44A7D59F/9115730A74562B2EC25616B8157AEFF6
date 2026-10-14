using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class AppSettingsService : BaseService, IAppSettingsService
{
    private readonly IAppSettingsCachingService _appSettingsCachingService;

    public AppSettingsService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IAppSettingsCachingService appSettingsCachingService) :
        base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _appSettingsCachingService = appSettingsCachingService;
    }

    public async Task<ServiceResult<List<AppSettingsGroup>>> Update()
    {
        var result = new ServiceResult<List<AppSettingsGroup>>();

        var keys = await (from ap in _context.AppSettings.AsNoTracking()
            orderby ap.Order
            select ap).ToListAsync();

        var groups = await (from apg in _context.AppSettingsGroups.AsNoTracking()
            orderby apg.Order
            select apg).ToListAsync();

        foreach (var g in groups)
        {
            g.Keys = keys.Where(k => k.GroupID == g.ID).ToList();
        }

        result.Data = groups;
        return result;
    }

    public async Task<ServiceResult> Update(List<AppSettingsGroup> request)
    {
        var result = new ServiceResult<List<AppSettingsGroup>>();

        var keys = await _context.AppSettings.ToListAsync();
        foreach (var group in request)
        {
            foreach (var key in group.Keys)
            {
                var dbKey = keys.FirstOrDefault(k => k.Key == key.Key);
                if (dbKey != null)
                {
                    dbKey.Value = key.Value;
                }
            }
        }

        await _context.SaveChangesAsync();

        await _appSettingsCachingService.RefreshAll();
        Quark.Business.Common.Caching.AppSettingsKeys.ClearLocalCache();

        result.SetSuccess(_localizer["RecordsUpdated"]);
        return result;
    }
}