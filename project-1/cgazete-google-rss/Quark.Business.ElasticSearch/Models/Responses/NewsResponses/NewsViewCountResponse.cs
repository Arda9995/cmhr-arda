using Nest;

namespace Quark.Business.ElasticSearch.Models.Responses.NewsResponses
{
    public class NewsViewCountResponse
    {
        [PropertyName("_id")]
        public string ID { get; set; }
        public int NewsID { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
