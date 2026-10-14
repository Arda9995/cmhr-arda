using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.SourceCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class SourceCachingService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), ISourceCachingService
{

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        var newsSourcesAsSelectListItems = await _context.Sources
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.NewsSourcesSelectListItems, newsSourcesAsSelectListItems);

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> NewsSources()
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

    public async Task<ServiceResult<List<GetSourceListResponse>>> RefreshNewsSourceList()
    {
        var result = new ServiceResult<List<GetSourceListResponse>>();
        var sources = await _context.Sources
             .Select(x => new GetSourceListResponse
             {
                 ID = x.ID,
                 Url = x.Url,
                 Name = x.Name,
                 Breadcrumb = x.Breadcrumb,
                 CreatedDate = x.CreatedDate
             }).ToListAsync();

        await _cache.Set(RedisKeys.NewsSourceList, sources);

        result.Data = sources;
        return result;
    }

    public async Task<ServiceResult<List<GetSourceListResponse>>> GetNewsSourceList()
    {
        var result = new ServiceResult<List<GetSourceListResponse>>();

        result.Data = await _cache.Get<List<GetSourceListResponse>>(RedisKeys.NewsSourceList);

        if (result.Data == null)
        {
            await RefreshNewsSourceList();
            result.Data = await _cache.Get<List<GetSourceListResponse>>(RedisKeys.NewsSourceList);
        }

        return result;
    }
}