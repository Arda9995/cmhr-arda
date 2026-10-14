using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.NewsStatusCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class NewsStatusCachingService: BaseService, INewsStatusCachingService
{
    public NewsStatusCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        var newsStatus = await _context.NewsStatus
            .Select(x => new NewsStatusCachingResponse
            {
                ID = x.ID,
                Name = x.Name,
                Value = x.Value,
                IsSelectableItem = x.IsSelectableItem,
            }).ToListAsync();
        await _cache.Set(RedisKeys.NewsStatus, newsStatus);
        
        var newsStatusSelectListItems = await _context.NewsStatus
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.NewsStatusSelectListItems, newsStatusSelectListItems);
        
        return result;
    }

    public async Task<ServiceResult<List<NewsStatusCachingResponse>>> GetAllNewsStatus()
    {
        var result = new ServiceResult<List<NewsStatusCachingResponse>>();

        result.Data = await _cache.Get<List<NewsStatusCachingResponse>>(RedisKeys.NewsStatus);
        
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<NewsStatusCachingResponse>>(RedisKeys.NewsStatus);
        }
        
        return result;
    }

    public async Task<ServiceResult<NewsStatusCachingResponse>> GetNewsStatusById(int id)
    {
        var result = new ServiceResult<NewsStatusCachingResponse>();

        var newsStatus = await _cache.Get<List<NewsStatusCachingResponse>>(RedisKeys.NewsStatus);
        
        if (newsStatus == null)
        {
            await RefreshAll();
            newsStatus = await _cache.Get<List<NewsStatusCachingResponse>>(RedisKeys.NewsStatus);        
        }
        
        result.Data = newsStatus.Where(x => x.ID == id).FirstOrDefault();
        
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetNewsStatusAsSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();
        
        result.Data = await _cache.Get_SelectListItems(RedisKeys.NewsStatusSelectListItems);

        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get_SelectListItems(RedisKeys.NewsStatusSelectListItems);
        }

        return result;
    }
}