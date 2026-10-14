using Quark.API.Business.Models.Responses.NewsResponses;
using Quark.Business.Caching.Models.Responses.PageCachingResponses;

namespace Quark.API.Business.Models.Responses.CommonResponses
{
	public class GetReadListPageResponse
	{
		public List<CategoryNews> CategoryNews { get; set; }
		public List<PageNewsCachingResponse> PageCachingResponses { get; set; }
	}
	public class ReadListItem
	{
		public int ID { get; set; }
		public string Headline { get; set; }
		public string Slug { get; set; }
		public string UpSubtitle { get; set; }
		public string ImageURL { get; set; }
		public string PublishDate { get; set; }
        public string SaveDate { get; set; }
    }
}
