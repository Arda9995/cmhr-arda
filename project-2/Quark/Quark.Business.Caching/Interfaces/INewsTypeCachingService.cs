using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.NewsTypeCachingResponses;

namespace Quark.Business.Caching.Interfaces;

public interface INewsTypeCachingService
{
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult<List<NewsTypeCachingResponse>>> GetAllNewsTypes();
    Task<ServiceResult<NewsTypeCachingResponse>> GetNewsTypeById(int id);
    Task<ServiceResult<List<SelectListItem>>> GetNewsTypesAsSelectListItems();
    Task<ServiceResult<List<SelectListItem>>> GetNewsSourcesAsSelectListItems();
}