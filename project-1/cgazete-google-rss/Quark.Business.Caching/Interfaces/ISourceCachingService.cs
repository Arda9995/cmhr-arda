using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.SourceCachingResponses;

namespace Quark.Business.Caching.Interfaces;

public interface ISourceCachingService
{
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult<List<SelectListItem>>> NewsSources();
    Task<ServiceResult<List<GetSourceListResponse>>> GetNewsSourceList();
    Task<ServiceResult<List<GetSourceListResponse>>> RefreshNewsSourceList();
}