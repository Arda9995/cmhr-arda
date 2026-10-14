using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.NewsPhotosCachingResponse;
using Quark.Business.Common.BaseServices;
using Quark.Business.ElasticSearch.Abstractions;
using Quark.Business.ElasticSearch.Constants;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class NewsPhotosCachingService : BaseService, INewsPhotosCachingService
{
    private readonly IElasticSearchService _elasticSearchService;
    
    public NewsPhotosCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IElasticSearchService elasticSearchService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _elasticSearchService = elasticSearchService;
    }
    

    public async Task<ServiceResult> RefreshNewsPublishedPhotoGalleryItems(int newsId)
    {
        var result = new ServiceResult();
        
        var newsPublishedPhotoGalleryItems = await _context.NewsPublishedPhotoGalleryItems
            .AsNoTracking()
            .Where(x => x.NewsID == newsId)
            .ToListAsync();

        if (!newsPublishedPhotoGalleryItems.Any())
        {
            result.SetError("Error - ElasticSearch update failed");
            return result;
        }

        var cacheModel = new GetNewsPublishedPhotoGalleryItemResponse()
        {
            ID = newsId,
            NewsPublishedPhotoGalleryItems = newsPublishedPhotoGalleryItems
        };
        
        var addElasticSearchResult = await _elasticSearchService.UpdateDocumentAsync(cacheModel,newsId.ToString(), IndexNames.NewsPublishedPhotoGalleryItem);
        if (!addElasticSearchResult)
        {
            result.SetError("Error - ElasticSearch update failed");
            return result;
        }
        
        return result;
    }

    public async Task<ServiceResult<List<NewsPublishedPhotoGalleryItem>>> GetNewsPublishedPhotoGalleryItems(int newsId)
    {
        var response = new ServiceResult<List<NewsPublishedPhotoGalleryItem>>();
        
        var cacheResponse = await _elasticSearchService.GetDocumentAsync<GetNewsPublishedPhotoGalleryItemResponse>(newsId.ToString(), IndexNames.NewsPublishedPhotoGalleryItem);
        if (cacheResponse == null)
        {
            await RefreshNewsPublishedPhotoGalleryItems(newsId);
            cacheResponse = await _elasticSearchService.GetDocumentAsync<GetNewsPublishedPhotoGalleryItemResponse>(newsId.ToString(), IndexNames.NewsPublishedPhotoGalleryItem);
        }
        
        response.Data = cacheResponse.NewsPublishedPhotoGalleryItems;
        response.IsSuccess = true;
        return response;
    }
}