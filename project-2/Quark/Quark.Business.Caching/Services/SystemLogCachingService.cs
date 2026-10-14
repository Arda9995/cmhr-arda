using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.SystemLogCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Data.Models.LogModels;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class SystemLogCachingService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), ISystemLogCachingService
{
    
    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();
        
        var homeLogs = await (from l in _context.SystemLogs
            join u in _context.Users on l.UserID equals u.ID
            join t in _context.LogTypes on l.LogTypeID equals t.ID
            where l.UserID != null && l.LogTypeID != (int)LogTypesEnum.Hangfire
            orderby l.ID descending
            select new SystemLogHomeCachingResponse
            {
                ID = l.ID,
                UserName = u.FullName,
                Message = l.LogTypeID == (int)LogTypesEnum.LogIn ? l.Message : t.Name + " " + l.Message,
                CreatedDate = l.CreatedDate.ToFormattedDateTime()
            }).Take(15).ToListAsync();
        await _cache.Set(RedisKeys.HomeLogs, homeLogs);
        
        var logTypesAsSelectListItems = await _context.LogTypes
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.LogTypesSelectListItems, logTypesAsSelectListItems);
        
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetLogTypesAsSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get_SelectListItems(RedisKeys.LogTypesSelectListItems);

        if (result.Data is null)
        {
            await RefreshAll();
            result.Data = await _cache.Get_SelectListItems(RedisKeys.LogTypesSelectListItems);
        }

        return result;
    }

    public async Task<ServiceResult<List<SystemLogHomeCachingResponse>>> GetHomeLogs()
    {
        var result = new ServiceResult<List<SystemLogHomeCachingResponse>>();

        result.Data = await _cache.Get<List<SystemLogHomeCachingResponse>>(RedisKeys.HomeLogs);

        if (result.Data is null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<SystemLogHomeCachingResponse>>(RedisKeys.HomeLogs);
        }
        
        return result;
    }
}