using Quark.Business.Caching.Models.Responses.NewsCachingResponses;

namespace Quark.Business.Caching.Models.Responses.AuthorCachingResponses
{
    public class GetAuthorNewsListCacheResponse
    {
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
        public string CategoryBreadcrumb { get; set; }
        public string CategoryColumnArticleSlug { get; set; }
        public List<CategoryNewsCacheResponse> CategoryNews { get; set; } = new List<CategoryNewsCacheResponse>();
        public List<TodaysNewsListCacheResponse> TodaysNewsList { get; set; } = new List<TodaysNewsListCacheResponse>();
        public List<VideoNewsCacheResponse> VideoNews { get; set; } = new List<VideoNewsCacheResponse>();
    }
}
