using Quark.Business.Caching.Models.Responses.PageCachingResponses;

namespace Quark.API.Business.Models.Responses.NewsResponses
{
	public class GetNewsListSearchedResponse
	{
        public string SearchKey { get; set; }
        public int TotalNewsCount { get; set; }
        public bool IsHaveMoreItems { get; set; } = true;
        public List<GetNewsListSearchResponse> NewsListSix { get; set; }
        public List<GetNewsListSearchResponse> NewsListOthers { get; set; }
        public List<CategoryNews> CategoryTopQuad { get; set; }
        public List<CategoryNews> CategoryBottomQuad { get; set; }
        public List<PageNewsCachingResponse> MostSearchedNews { get; set; }

	}
}
