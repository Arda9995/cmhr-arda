namespace Quark.Business.Caching.Models.Responses.SourceCachingResponses
{
    public class GetSourceListResponse
    {
        public int ID { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Breadcrumb { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
