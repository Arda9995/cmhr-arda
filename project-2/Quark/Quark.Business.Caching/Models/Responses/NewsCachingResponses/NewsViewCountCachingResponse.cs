namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses
{
	public class NewsViewCountCachingResponse
	{
        public int NewsID { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long ViewCount { get; set; }
    }
}
