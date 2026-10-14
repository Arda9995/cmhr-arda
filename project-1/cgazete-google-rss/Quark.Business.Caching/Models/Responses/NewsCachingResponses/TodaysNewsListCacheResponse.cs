namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses
{
    public class TodaysNewsListCacheResponse
    {
        public int NewsID { get; set; }
        public string Slug { get; set; }
        public string Photo { get; set; }
        public string Headline { get; set; }
        public string AuthorName { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
