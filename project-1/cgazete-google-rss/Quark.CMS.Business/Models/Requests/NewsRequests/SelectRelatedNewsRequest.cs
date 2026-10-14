namespace Quark.CMS.Business.Models.Requests.NewsRequests
{
    public class SelectRelatedNewsRequest
    {
        public long NewsID { get; set; }
        public long RelatedNewsID { get; set; }
    }
}