namespace Quark.API.Business.Models.Responses.NewsResponses
{
	public class GetNewsListSearchResponse
	{
        public int ID { get; set; }
        public string Headline { get; set; }
        public string UpSubtitle { get; set; }
        public string Slug { get; set; }
        public string ImageURL { get; set; }
        public string PublishDate { get; set; }
    }
}
