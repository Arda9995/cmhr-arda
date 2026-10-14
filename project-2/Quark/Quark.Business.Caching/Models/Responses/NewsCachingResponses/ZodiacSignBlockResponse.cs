using Quark.Data.Models;

namespace Quark.API.Caching.Models.Responses.NewsCachingResponses
{
    public class ZodiacSignBlockCacheResponse
    {
        public List<ZodiacSignBlockCacheResponseItem> ZodiacSignItems { get; set; } = new List<ZodiacSignBlockCacheResponseItem>();
        public string AstrologerName { get; set; }
        public string AstrologerImageURL { get; set; }
        public string DateStringForZodiacSignDateRange { get; set; }
    }
    public class ZodiacSignBlockCacheResponseItem
    {
        public long NewsID { get; set; }
        public string Slug { get; set; }
        public string Icon { get; set; }
        public DateTime PublishDate { get; set; }
        public ZodiacSignEnum ZodiacSignType { get; set; }
    }
}
