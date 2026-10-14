using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.NewsStatusCachingResponses;

namespace Quark.Business.Caching.Interfaces;

public interface INewsStatusCachingService
{
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult<List<NewsStatusCachingResponse>>> GetAllNewsStatus();
    Task<ServiceResult<NewsStatusCachingResponse>> GetNewsStatusById(int id);
    Task<ServiceResult<List<SelectListItem>>> GetNewsStatusAsSelectListItems();
}