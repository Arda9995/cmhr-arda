using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.PromptCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class PromptCachingService: BaseService, IPromptCachingService
{

    public PromptCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        var promptTypesAsSelectListItems = await _context.PromptTypes
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            })
            .ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.PromptTypesSelectListItems, promptTypesAsSelectListItems);

        // var prompts = await _context.Prompts
        //     .Select(x => new PromptCachingResponse
        //     {
        //         ID = x.ID,
        //         Name = x.Name,
        //         Message = x.Message,
        //         PromptTypeID = x.PromptTypeID
        //     }).ToListAsync();

        var prompts = await (from p in _context.Prompts
            join rt in _context.RequestTypes on p.RequestTypeID equals rt.ID
            select new PromptCachingResponse
            {
                ID = p.ID,
                Name = p.Name,
                Message = p.Message,
                PromptTypeID = p.PromptTypeID,
                RequestType = rt.Name
            }).ToListAsync();
        await _cache.Set(RedisKeys.Prompts, prompts);
        
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetPromptTypesAsSelectListItems()
    {
        var result = new ServiceResult<List<SelectListItem>>();
        
        result.Data = await _cache.Get_SelectListItems(RedisKeys.PromptTypesSelectListItems);
        
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get_SelectListItems(RedisKeys.PromptTypesSelectListItems);
        }
        
        return result;    
    }

    public async Task<ServiceResult<List<PromptCachingResponse>>> GetPrompts()
    {
        var result = new ServiceResult<List<PromptCachingResponse>>();

        result.Data = await _cache.Get<List<PromptCachingResponse>>(RedisKeys.Prompts);

        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<PromptCachingResponse>>(RedisKeys.Prompts);
        }
        
        return result;
    }

    public async Task<ServiceResult<PromptCachingResponse>> GetPromptByID(int id)
    {
        var result = new ServiceResult<PromptCachingResponse>();

        var prompts = await _cache.Get<List<PromptCachingResponse>>(RedisKeys.Prompts);
        if (prompts == null)
        {
            await RefreshAll();
        }

        result.Data = prompts.FirstOrDefault(x => x.ID == id);
        
        return result;
    }
}