using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.NewsPhotoTypeCachingResponses;

namespace Quark.Business.Caching.Interfaces
{
    public interface INewsPhotoTypeCachingService
    {
        Task<ServiceResult> RefreshAll();
        Task<ServiceResult<List<NewsPhotoTypeCachingResponse>>> Get();
        Task<ServiceResult<List<SelectListItem>>> GetSelectListItems();
    }
}