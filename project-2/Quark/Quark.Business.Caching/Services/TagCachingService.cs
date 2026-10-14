using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.TagCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Business.ElasticSearch.Abstractions;
using Quark.Business.ElasticSearch.Constants;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;

namespace Quark.Business.Caching.Services;

public class TagCachingService : BaseService, ITagCachingService
{
    private readonly IElasticSearchService _elasticSearchService;
    public TagCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, IElasticSearchService elasticSearchService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _elasticSearchService = elasticSearchService;
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult<List<string>>();

        var tags = await _context.Tags.AsNoTracking().Select(x => new TagCachingResponse
        {
            ID = x.ID,
            Text = x.Text,
            Slug = x.Url
        }).ToListAsync();

        await _cache.Set(RedisKeys.TagsKey, tags);

        return result;
    }

    public async Task<ServiceResult<List<TagCachingResponse>>> Get()
    {
        var result = new ServiceResult<List<TagCachingResponse>>();

        result.Data = await _cache.Get<List<TagCachingResponse>>(RedisKeys.TagsKey);
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<TagCachingResponse>>(RedisKeys.TagsKey);
        }

        return result;
    }    
    
    public async Task<ServiceResult<TagCachingResponse>> GetByText(string text)
    {
        var result = new ServiceResult<TagCachingResponse>();

        if(text != null)
        {
            var tags = await _cache.Get<List<TagCachingResponse>>(RedisKeys.TagsKey);
            if (tags == null)
            {
                await RefreshAll();
                tags = await _cache.Get<List<TagCachingResponse>>(RedisKeys.TagsKey);
            }

            result.Data = tags.Where(x => x.Text.Contains(text)).FirstOrDefault();
        }
        return result;
    }

    public async Task<ServiceResult> RefreshAllTagInElastic()
    {
        var result = new ServiceResult();
        
        var tags = await _context.Tags.AsNoTracking().ToListAsync();
        if (!tags.Any())
        {
            return result;
        }

        var addElastic = _elasticSearchService.BulkIndexDocumentsAsync(tags,IndexNames.Tags);

        return result;
    }

    public async Task<ServiceResult<List<Tag>>> GetTagsElastic(List<int> ids)
    {
        var result = new ServiceResult<List<Tag>>();
        
        var tags = await _elasticSearchService.GetByIdsDocumentAsync<Tag>(ids.Select(x => x.ToString()).ToList(), IndexNames.Tags);
        
        if (!tags.Any())
        {
            await RefreshAllTagInElastic();
            tags = await _elasticSearchService.GetByIdsDocumentAsync<Tag>(ids.Select(x => x.ToString()).ToList(), IndexNames.Tags);
        }
        
        result.Data = tags;
        return result;
    }

    public async Task<ServiceResult<bool>> UpdateTagElastic(Tag tag)
    {
        var result = new ServiceResult<bool>();

        var updateElastic = _elasticSearchService.UpdateDocumentAsync(tag, tag.ID.ToString() ,IndexNames.Tags);
        result.Data = await updateElastic;
        
        return result;
    }

    public async Task<ServiceResult<bool>> InsertTagElastic(Tag tag)
    {
        var result = new ServiceResult<bool>();

        var insertElastic = _elasticSearchService.IndexDocumentAsync(tag, IndexNames.Tags);
        result.Data = await insertElastic;
        
        return result;
    }
}