using System.Text.Json.Nodes;
using AvvaMobile.Core.Business;
using GoogleTrendsApi;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.CMS.Business.Models.Responses.GoogleTrendsResponses;

namespace Quark.CMS.Business.Interfaces;

public interface IGoogleTrendsService
{
    Task<ServiceResult<RealtimeSearchResponse>> GetRealtimeSearches(Category category);
    ServiceResult<List<SelectListItem>> GetCategories();
}