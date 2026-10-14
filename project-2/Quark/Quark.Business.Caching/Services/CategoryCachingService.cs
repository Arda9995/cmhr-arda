using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.CategoryCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Business.ElasticSearch.Abstractions;
using Quark.Business.ElasticSearch.Constants;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class CategoryCachingService : BaseService, ICategoryCachingService
{
    private readonly IElasticSearchService _elasticSearchService;

    public CategoryCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IElasticSearchService elasticSearchService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _elasticSearchService = elasticSearchService;
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        var newsCategories = await _context.Categories.Select(x => new CategoryCachingResponse
        {
            ID = x.ID,
            Name = x.Name,
            Breadcrumb = x.Breadcrumb,
            Map = x.Map,
            Url = x.Url,
            IsEnabled = x.IsEnabled,
            PhotoUrl = x.PhotoUrl,
            SeoDescription = x.SeoDescription,
            SeoTitle = x.SeoTitle,
            ParentID = x.ParentID,
        }).ToListAsync();
        await _cache.Set(RedisKeys.AllCategories, newsCategories);

        var newsCategoriesSelectListItems = await _context.Categories
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.CategoriesSelectListItems, newsCategoriesSelectListItems);

        return result;
    }

    public async Task<ServiceResult> DeleteCategoryNewsList(int id)
    {
        var result = new ServiceResult();

        var categoryIds = new List<string>()
        {
            id.ToString()
        };

        var deleteDocument =
            await _elasticSearchService.DeleteIndexDocuments(IndexNames.NewsListByCategorySlug, categoryIds);

        await _cache.Remove(string.Format(RedisKeys.NewsByCategorySlug, id));

        return result;
    }

    public async Task<ServiceResult<List<CategoryCachingResponse>>> GetAllCategories()
    {
        var result = new ServiceResult<List<CategoryCachingResponse>>();

        result.Data = await _cache.Get<List<CategoryCachingResponse>>(RedisKeys.AllCategories);

        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<CategoryCachingResponse>>(RedisKeys.AllCategories);
        }

        return result;
    }

    public async Task<ServiceResult<CategoryCachingResponse>> GetCategoryById(int id)
    {
        var result = new ServiceResult<CategoryCachingResponse>();

        var categories = await _cache.Get<List<CategoryCachingResponse>>(RedisKeys.Categories);

        if (categories == null)
        {
            await RefreshAll();
            categories = await _cache.Get<List<CategoryCachingResponse>>(RedisKeys.Categories);
        }

        result.Data = categories.Where(x => x.ID == id).FirstOrDefault();

        return result;
    }

    public async Task<ServiceResult<CategoryCachingResponse>> GetCategoryByName(string name)
    {
        var result = new ServiceResult<CategoryCachingResponse>();

        if (name != null)
        {
            var categories = await _cache.Get<List<CategoryCachingResponse>>(RedisKeys.Categories);

            if (categories == null)
            {
                await RefreshAll();
                categories = await _cache.Get<List<CategoryCachingResponse>>(RedisKeys.Categories);
            }

            result.Data = categories.Where(x => x.Name.Contains(name)).FirstOrDefault();
        }
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetCategoriesAsSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get_SelectListItems(RedisKeys.CategoriesSelectListItems);

        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get_SelectListItems(RedisKeys.CategoriesSelectListItems);
        }

        return result;
    }
    
    
}