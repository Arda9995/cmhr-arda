namespace Quark.Business.Caching.Models.Responses.PageCachingResponses
{
	public class PageCachingResponse
	{
		public int ID { get; set; }
		public string Key { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public string Description { get; set; }
		public string Content { get; set; }
		public string Breadcrumb { get; set; }
	}
}
