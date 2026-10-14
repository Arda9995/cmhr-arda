using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Requests.PageCachingRequests;
using Quark.Business.Caching.Models.Responses.PageCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;

using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class PageCachingService : BaseService, IPageCachingService
{
    // private const string key = "_Content_Pages_{0}";
    // private const string pagesKey = "Pages_Web";
    // private const string _keyPagesSelectListItems = "Pages_SelectListItems";

    public PageCachingService(IHttpContextAccessor httpContext, QuarkDataContext context,
        IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(
        httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult<PageCachingResponse>> GetByKey(PageCachingRequest request)
    {
        var result = new ServiceResult<PageCachingResponse>();

        result.Data = await _cache.Get<PageCachingResponse>(string.Format(RedisKeys.ContentPages, request.Key));
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<PageCachingResponse>(string.Format(RedisKeys.ContentPages, request.Key));
        }

        return result;
    }

    public async Task<ServiceResult<List<PageWebCachingResponse>>> GetWebPages()
    {
        var result = new ServiceResult<List<PageWebCachingResponse>>();

        result.Data = await _cache.Get<List<PageWebCachingResponse>>(RedisKeys.WebPages);
        if (result.Data == null)
        {
            await RefreshPagesSlugs();
            result.Data = await _cache.Get<List<PageWebCachingResponse>>(RedisKeys.WebPages);
        }

        return result;
    }

    public async Task<ServiceResult> RefreshCache(int id)
    {
        var result = new ServiceResult();

        var data = await (from b in _context.Pages
                          where b.ID == id
                          select new PageCachingResponse
                          {
                              ID = b.ID,
                              Key = b.Key,
                              Name = b.Name,
                              Description = b.Description,
                              Content = b.Content,
                              Slug = b.Slug,
                              Breadcrumb = b.Breadcrumb,
                          }).AsNoTracking().FirstOrDefaultAsync();

        await _cache.Set(string.Format(RedisKeys.ContentPages, data.Key), data);

        return result;
    }

    public async Task<ServiceResult> RefreshPagesSlugs()
    {
        var result = new ServiceResult();

        var data = await (from b in _context.Pages
                          select new PageWebCachingResponse
                          {
                              Key = b.Key,
                              Slug = b.Slug,
                          }).AsNoTracking().ToListAsync();
        await _cache.Set(RedisKeys.WebPages, data);

        return result;
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        var items = await _context.Pages.AsNoTracking().ToListAsync();

        foreach (var item in items)
        {
            await RefreshCache(item.ID);
        }
        await RefreshPagesSlugs();

		var pagesAsSelectListItems = await _context.Pages
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.PagesSelectListItems, pagesAsSelectListItems);

        return result;
    }

    public async Task<ServiceResult> RemoveCache(string listKey)
    {
        var result = new ServiceResult();

        await _cache.Remove(string.Format(RedisKeys.ContentPages, listKey));

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> PagesAsSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get_SelectListItems(RedisKeys.PagesSelectListItems);

        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get_SelectListItems(RedisKeys.PagesSelectListItems);
        }
        return result;
    }

    public async Task<ServiceResult<PageCachingResponse>> GetBySlug(string slug)
    {
        var result = new ServiceResult<PageCachingResponse>();

        var page = string.Empty;
        var pageKey = await GetWebPages();

        if (pageKey.Data != null)
        {
            page = pageKey.Data.Where(x => x.Slug == slug).Select(x=>x.Key).FirstOrDefault();

            result.Data = await _cache.Get<PageCachingResponse>(string.Format(RedisKeys.ContentPages, page));
        }

        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<PageCachingResponse>(string.Format(RedisKeys.ContentPages, page));
        }

        return result;
    }
}