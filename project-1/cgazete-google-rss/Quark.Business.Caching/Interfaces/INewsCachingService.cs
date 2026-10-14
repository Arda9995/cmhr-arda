using AvvaMobile.Core.Business;
using Quark.API.Caching.Models.Responses.NewsCachingResponses;
using Quark.Business.Caching.Models.Responses.AuthorCachingResponses;
using Quark.Business.Caching.Models.Responses.NewsCachingResponses;
using Quark.Data.Models;
using Quark.Data.Models.News;

namespace Quark.Business.Caching.Interfaces;

public interface INewsCachingService
{
    Task<ServiceResult> RefreshAll();

    #region SURVEYS

    Task<ServiceResult> RefreshSurveyResult(int surveyID);
    Task<ServiceResult> IncreaseSurveyResult(int surveyID, int optionID);
    Task<ServiceResult<SurveyOptionResultCachingResponse>> GetSurveyOptionResult(int surveyID);

    #endregion

    #region VIEW COUNT
    
    Task<ServiceResult> IncreaseNewsViewCount(int newsID);

    #endregion

    #region MAINPAGE

    Task<ServiceResult> RefreshMainPageGovernmentAnnouncement();
    Task<ServiceResult<List<GovernmentAnnouncementItem>>> GetMainPageGovernmentAnnouncement();

    Task<ServiceResult> RefreshMainPageZodiacSignBlock();
    Task<ServiceResult<ZodiacSignBlockCacheResponse>> GetMainPageZodiacSignBlock();

    #endregion

    #region AUTHOR
    
    Task<ServiceResult> RefreshAuthorSideColumnArticles();
    Task<ServiceResult<List<CacheTodaysColumnArticles>>> GetAuthorSideColumnArticles();

    Task<ServiceResult> RefreshAuthorDetailLastColumnArticles(string slug);
    Task<ServiceResult<GetAuthorLastColumnArticlesCacheResponse>> GetAuthorDetailLastColumnArticles(string slug);
    Task<ServiceResult> RefreshAuthorLastNews(int authorID);
    Task<ServiceResult<List<AuthorsLastColumnArticlesCacheResponse>>> GetAuthorLastNews(int authorID);

    #endregion

    #region CATEGORY, TAG, SOURCES

    Task<ServiceResult> RefreshNewsBySourceIds(List<int> ids);
    Task<ServiceResult> RefreshNewsBySourceSlug(string sourceSlug);
    Task<ServiceResult> RefreshNewsByCategorySlug(int categoryID);
    Task<ServiceResult> RefreshNewsByTagSlug(string tagSlug);
    Task<ServiceResult<GetNewsListCachingResponse>> GetCachingNewsByCategorySlug(int categoryID);
    Task<ServiceResult<GetNewsListCachingResponse>> GetCachingNewsBySourceSlug(string sourceSlug);
    Task<ServiceResult<GetNewsListCachingResponse>> GetCachingNewsByTagSlug(string tagSlug);

    #endregion

    #region NEWS

    Task<ServiceResult<GetCachingNewsDetailResponse>> CreateNewsDetail(int newsId);
    Task<ServiceResult> RefreshMostViewNews();
    Task<ServiceResult<List<MostViewNewsCachingResponse>>> GetMostViewNews();
    Task<ServiceResult> RefreshNewsDetails(int id);
    Task<ServiceResult<GetCachingNewsDetailResponse>> GetNewsDetailByID(int id);
    Task<ServiceResult> DeleteNewsPublishedSearchByID(int id);

    Task<ServiceResult> RefreshNewsPublishedSearch();
    Task<ServiceResult> RefreshNewsPublishedSearchByID(int id);
    Task<ServiceResult<List<GetNewsPublishedCacheResponse>>> GetNewsPublishedSearch(string keyword, int size = 10000);
    Task<ServiceResult<List<int>>> GetNewsIdsByNewsType(int newsTypeID, int size = 10000);

    #endregion

    #region ETC

    Task<ServiceResult> RefreshAllNewsTags();

    #endregion

    Task<ServiceResult<List<GetIncreasedNewsViewCountResponse>>> GetIncreasedNewsViewCount();

    Task HandlePublishCacheOperations(News news);
    Task HandleUnPublishCacheOperations(News news);
}