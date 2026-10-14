using Newtonsoft.Json;
using Quark.API.Business.Models.Responses.AuthorResponses;
using Quark.API.Business.Models.Responses.NewsResponses;
using Quark.Business.Caching.Models.Responses.AuthorCachingResponses;
using Quark.Business.Caching.Models.Responses.BookCachingResponses;
using Quark.Business.Caching.Models.Responses.NewsCachingResponses;
using Quark.Business.Caching.Models.Responses.PageCachingResponses;
using Quark.Data.Models;

namespace Quark.API.Business.Models.Responses.PageResponses
{
	public class MainPageResponse
	{
        public List<PageNewsCachingResponse>  PageNewsCachingResponses { get; set; }
        public List<GetAuthorListResponse> AuthorCachingResponse { get; set; }
        public List<BookCachingResponse> BookCachingResponses { get; set; }
        public List<ZodiacSignBlock> ZodiacSignItems { get; set; }
        [JsonIgnore]
        public List<GovernmentAnnouncementItem> GovernmentAnnouncementItems { get; set; }
        public string AstrologerName{ get; set; }
        public string AstrologerImageURL{ get; set; }
        public string AstrologieDateStr { get; set; }

    }
    public class GovernmentFilledBlock
    {
        public List<PageNewsCachingResponse> FillerBlockNews { get; set; }
        public List<GovernmentAnnouncementItem> GovernmentAnnouncementItems{ get; set; }
    }

    public class ZodiacSignBlock
    {
        public long NewsID{ get; set; }
        public string Slug{ get; set; }
        public string Icon{ get; set; }
        public DateTime PublishDate{ get; set; }
        public ZodiacSignEnum ZodiacSignType{ get; set; }
    }
}
