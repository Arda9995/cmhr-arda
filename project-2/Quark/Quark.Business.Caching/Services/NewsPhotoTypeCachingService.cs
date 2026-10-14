using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.NewsPhotoTypeCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;

namespace Quark.Business.Caching.Services;

public class NewsPhotoTypeCachingService : BaseService, INewsPhotoTypeCachingService
{
    public NewsPhotoTypeCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        var newsPhotoTypesSelectListItem = await _context.NewsPhotoTypes
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();

        await _cache.Set_SelectListItems(RedisKeys.NewsPhotoTypesSelectListItems, newsPhotoTypesSelectListItem);

        var newsPhotoTypes = _context.NewsPhotoTypes.Select(item => new NewsPhotoTypeCachingResponse
        {
            ID = item.ID,
            Key = item.Key,
            Name = item.Name,
            Description = item.Description,
            Width = item.Width,
            Height = item.Height,
            IsRequired = item.IsRequired,
            Order = item.Order,
            IsEnabled = item.IsEnabled,
        }).ToList();

        await _cache.Set(RedisKeys.NewsPhotoTypes, newsPhotoTypes);

        return result;
    }

    public async Task<ServiceResult<List<NewsPhotoTypeCachingResponse>>> Get()
    {
        var result = new ServiceResult<List<NewsPhotoTypeCachingResponse>>();

        result.Data = await _cache.Get<List<NewsPhotoTypeCachingResponse>>(RedisKeys.NewsPhotoTypes);
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<NewsPhotoTypeCachingResponse>>(RedisKeys.NewsPhotoTypes);
        }

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get<List<SelectListItem>>(RedisKeys.NewsPhotoTypesSelectListItems);
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<SelectListItem>>(RedisKeys.NewsPhotoTypesSelectListItems);
        }

        return result;
    }
}