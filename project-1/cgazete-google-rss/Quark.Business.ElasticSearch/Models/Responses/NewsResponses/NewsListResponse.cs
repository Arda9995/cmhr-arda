namespace Quark.Business.ElasticSearch.Models.Responses.NewsResponses
{
    public class NewsListResponse
    {
        public int TotalCount { get; set; }
        
        public List<NewsListItemResponse> News { get; set; }
    }
}
