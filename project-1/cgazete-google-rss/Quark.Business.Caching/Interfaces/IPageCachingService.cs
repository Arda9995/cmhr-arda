using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Requests.PageCachingRequests;
using Quark.Business.Caching.Models.Responses.PageCachingResponses;

namespace Quark.Business.Caching.Interfaces
{
    public interface IPageCachingService
    {
        Task<ServiceResult<PageCachingResponse>> GetByKey(PageCachingRequest request);
        Task<ServiceResult<PageCachingResponse>> GetBySlug(string slug);
        Task<ServiceResult> RefreshCache(int id);
        Task<ServiceResult> RefreshAll();
        Task<ServiceResult> RefreshPagesSlugs();
        Task<ServiceResult> RemoveCache(string listKey);
        Task<ServiceResult<List<SelectListItem>>> PagesAsSelectListItems();
        Task<ServiceResult<List<PageWebCachingResponse>>> GetWebPages();

    }
}
