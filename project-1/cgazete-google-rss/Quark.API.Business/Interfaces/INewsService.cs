using AvvaMobile.Core.Business;
using Quark.API.Business.Models.Responses.NewsResponses;
using Quark.API.Caching.Models.Responses.NewsCachingResponses;
using Quark.Business.Caching.Models.Responses.NewsCachingResponses;
using Quark.Business.Caching.Models.Responses.PageCachingResponses;

namespace Quark.API.Business.Interfaces
{
    public interface INewsService
    {
        Task<ServiceResult> RemoveNewsByIDList(long[] IDList);
        Task<ServiceResult> TestMethod();
        Task<ServiceResult<GetNewsDetailResponse>> GetNewsDetail(int ID);
        Task<ServiceResult<int>> CheckCategoryTypeBySlug(string slug);
        Task<ServiceResult<GetNewsListResponse>> GetNewsByCategorySlug(int categoryID);
        Task<ServiceResult<GetNewsListResponse>> GetMobileNewsByCategorySlug(int categoryID);
        Task<ServiceResult<GetNewsListResponse>> GetNewsBySourceSlug(string sourceSlug);
        Task<ServiceResult<GetNewsListResponse>> GetNewsByTagSlug(string tagSlug);
        Task<ServiceResult<int>> GetNewsType(int newsID);
        Task<ServiceResult<string>> CheckURLToRedirect(string slug, string contextURL);
        Task<ServiceResult<ZodiacSignBlockCacheResponse>> GetZodiacSignNewsListForIndex();
        Task<ServiceResult<List<GetNewsListSearchResponse>>> GetNewsListBySearchKey(string search);
        Task<ServiceResult<GetNewsListSearchedResponse>> GetNewsListBySearchedKey(string search);
        Task<ServiceResult<List<PageNewsCachingResponse>>> GetMostSearchedNewsBlock();
        Task<ServiceResult<LoadMoreNewsResponse>> GetMoreNewsByCategoryId(int categoryId, int page, int pageSize);
        Task<ServiceResult<LoadMoreNewsResponse>> GetMoreNewsBySourceSlug(string sourceSlug, int page, int pageSize);
        Task<ServiceResult<LoadMoreNewsResponse>> GetMoreNewsByTagSlug(string tagSlug, int page, int pageSize);
        Task<ServiceResult<LoadMoreSearchResponse>> GetMoreNewsBySearchKey(string search, int page, string dateRange);
        Task<ServiceResult> VoteSurvey(int surveyId, int optionId);
        Task<ServiceResult<SurveyOptionResultCachingResponse>> GetSurveyOptionResult(int surveyID);
        Task<ServiceResult<string>> GetUrlByID(int id);
        Task<ServiceResult<List<GovernmentAnnouncementItem>>> GetDailyGovernmentAnnouncement();
        Task<ServiceResult<List<GovernmentAnnouncementItem>>> GetGovernmentAnnouncementListByDate(DateTime dateTime);
        Task<ServiceResult> AddViewCount(int newsID);
        Task<ServiceResult<GetCachingNewsDetailResponse>> CreateNewsDetail(int id);
    }
}
