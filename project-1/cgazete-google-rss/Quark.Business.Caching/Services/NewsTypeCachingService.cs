using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.NewsTypeCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class NewsTypeCachingService : BaseService, INewsTypeCachingService
{
    public NewsTypeCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        var newsTypes = await _context.NewsTypes
            .OrderBy(x => x.Order)
            .Select(x => new NewsTypeCachingResponse
            {
                ID = x.ID,
                Name = x.Name,
                Order = x.Order,
                Icon = x.Icon
            }).ToListAsync();
        await _cache.Set(RedisKeys.NewsTypes, newsTypes);
        
        var newsTypesSelectListItems = await _context.NewsTypes
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.NewsTypesSelectListItems, newsTypesSelectListItems);        
        
        var newsSourcesSelectListItems = await _context.Sources
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.NewsSourcesSelectListItems, newsSourcesSelectListItems);
        
        return result;
    }

    public async Task<ServiceResult<List<NewsTypeCachingResponse>>> GetAllNewsTypes()
    {
        var result = new ServiceResult<List<NewsTypeCachingResponse>>();

        result.Data = await _cache.Get<List<NewsTypeCachingResponse>>(RedisKeys.NewsTypes);
        
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<NewsTypeCachingResponse>>(RedisKeys.NewsTypes);
        }
        
        return result;
    }

    public async Task<ServiceResult<NewsTypeCachingResponse>> GetNewsTypeById(int id)
    {
        var result = new ServiceResult<NewsTypeCachingResponse>();

        var newsTypes = await _cache.Get<List<NewsTypeCachingResponse>>(RedisKeys.NewsTypes);
        
        if (newsTypes == null)
        {
            await RefreshAll();
            newsTypes = await _cache.Get<List<NewsTypeCachingResponse>>(RedisKeys.NewsTypes);        
        }
        
        result.Data = newsTypes.Where(x => x.ID == id).FirstOrDefault();
        
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetNewsTypesAsSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();
        
        result.Data = await _cache.Get_SelectListItems(RedisKeys.NewsTypesSelectListItems);

        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get_SelectListItems(RedisKeys.NewsTypesSelectListItems);
        }

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetNewsSourcesAsSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get_SelectListItems(RedisKeys.NewsSourcesSelectListItems);

        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get_SelectListItems(RedisKeys.NewsSourcesSelectListItems);
        }

        return result;
    }
}