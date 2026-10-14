using AvvaMobile.Core.Business;
using Quark.API.Business.Models.Responses.AuthorResponses;
using Quark.API.Business.Models.Responses.NewsResponses;
using Quark.Business.Caching.Models.Requests.AuthorCachingRequests;

namespace Quark.API.Business.Interfaces
{
    public interface IAuthorService
    {
        Task<ServiceResult<List<GetAuthorListResponse>>> GetAuthorList();
        Task<ServiceResult<AuthorDetailResponse>> GetAuthorDetail(string slug);
		Task<ServiceResult<List<GetAuthorListResponse>>> GetHomePageAuthorList();
		Task<ServiceResult<GetAuthorNewsListResponse>> GetAuthorNewsTodayList();
		Task<ServiceResult<GetAuthorNewsListResponse>> GetAuthorCategoryNewsTodayList(string slug);
        Task<ServiceResult<object>> GetFilteredArticlesWithPagination(int authorId, string dateRange, int page = 1, int pageSize = 40);
	}
}
