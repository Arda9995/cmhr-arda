using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class AnomalyCachingService : BaseService, IAnomalyCachingService
{

    public AnomalyCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();
        
        var anomalyDataTypesAsSelectListItems = await _context.AnomalyDataTypes
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.AnomalyDataTypesSelectListItems, anomalyDataTypesAsSelectListItems);
        
        var anomalyIssueTypesAsSelectListItems = await _context.AnomalyIssueTypes
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.AnomalyIssueTypesSelectListItems, anomalyIssueTypesAsSelectListItems);
        
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> AnomalyDataTypesAsSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();
        
        result.Data = await _cache.Get_SelectListItems(RedisKeys.AnomalyDataTypesSelectListItems);
        
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get_SelectListItems(RedisKeys.AnomalyDataTypesSelectListItems);
        }
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> AnomalyIssueTypesAsSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();
        
        result.Data = await _cache.Get_SelectListItems(RedisKeys.AnomalyIssueTypesSelectListItems);
        
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get_SelectListItems(RedisKeys.AnomalyIssueTypesSelectListItems);
        }
        return result;
    }
}