using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using GoogleTrendsApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Responses.GoogleTrendsResponses;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class GoogleTrendsService: BaseService, IGoogleTrendsService
{
    public GoogleTrendsService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult<RealtimeSearchResponse>> GetRealtimeSearches(Category category)
    {
        var result = new ServiceResult<RealtimeSearchResponse> { Data = new RealtimeSearchResponse() };
        // try
        // {
        //     var jsonArray = await Api.GetRealtimeSearches("TR", category, 20); 
        //     string jsonString = jsonArray.ToString(); 
        //
        //     List<TrendData> trendDataList = JsonConvert.DeserializeObject<List<TrendData>>(jsonString);
        //
        //     result.Data.TrendsList = trendDataList;
        //
        // }
        // catch (Exception e)
        // {
        //     result.Data.TrendsList = new List<TrendData>();
        //     result.SetError("Google trend haberlerine ula��lamad�");
        // }

        return result;
    }
    
    public ServiceResult<List<SelectListItem>> GetCategories()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        // var selectList = new List<SelectListItem>
        // {
        //     new SelectListItem { Value = nameof(Category.All), Text = _localizer["All"], Selected = true},
        //     new SelectListItem { Value = nameof(Category.Entertainment), Text = _localizer["Entertainment"] },
        //     new SelectListItem { Value = nameof(Category.Business), Text = _localizer["Business"] },
        //     new SelectListItem { Value = nameof(Category.ScienceAndTech), Text = _localizer["ScienceAndTech"] },
        //     new SelectListItem { Value = nameof(Category.Health), Text = _localizer["Health"] },
        //     new SelectListItem { Value = nameof(Category.Sports), Text = _localizer["Sports"] },
        //     new SelectListItem { Value = nameof(Category.TopStories), Text = _localizer["TopStories"] }
        // };
        //
        // result.Data = selectList;
        
        return result;
    }
}