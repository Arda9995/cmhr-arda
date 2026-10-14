using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.AuthorCachingResponses;
using Quark.Data.Models;

namespace Quark.Business.Caching.Interfaces
{
    public interface IAuthorCachingService
    {
        Task<ServiceResult> RefreshAll();
        Task<ServiceResult> RefreshAuthorDetailCaching();
        Task<ServiceResult> RefreshAuthors();
        Task<ServiceResult> RefreshMainPageAuthorsAll();
        Task<ServiceResult<List<AuthorCachingResponse>>> GetAll();
        Task<ServiceResult<List<AuthorCachingResponse>>> GetMainPage();
        Task<ServiceResult<List<SelectListItem>>> GetAsSelectList();
        Task<ServiceResult<List<Author>>> GetAuthors();
		Task<ServiceResult> RefreshAuthorNewsTodayList();
		Task<ServiceResult<GetAuthorNewsListCacheResponse>> GetAuthorNewsTodayList();
        Task<ServiceResult> RefreshAuthorCategoryNewsTodayList(string slug);
        Task<ServiceResult<GetAuthorNewsListCacheResponse>> GetAuthorCategoryNewsTodayList(string slug);
    }
}