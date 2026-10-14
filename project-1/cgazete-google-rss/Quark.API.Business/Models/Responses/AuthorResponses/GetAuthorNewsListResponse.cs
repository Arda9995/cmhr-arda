using Quark.API.Business.Models.Responses.NewsResponses;

namespace Quark.API.Business.Models.Responses.AuthorResponses
{
	public class GetAuthorNewsListResponse
	{
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
        public string CategoryBreadcrumb { get; set; }
        public string CategoryColumnArticleSlug { get; set; }
        public List<CategoryNews> CategoryNews { get; set; } = new List<CategoryNews>();
        public List<TodaysNewsList> TodaysNewsList { get; set; } = new List<TodaysNewsList>();
        public List<VideoNews> VideoNews { get; set; } = new List<VideoNews>();
    }
    public class VideoNews
    {
        public string Slug { get; set; }
        public string Headline { get; set; }
        public string ImageURL { get; set; }
        public int Minute { get; set; }
    }

	public class TodaysNewsList
	{
        public int NewsID { get; set; }
        public string Slug { get; set; }
        public string Photo { get; set; }
        public string Headline { get; set; }
        public string AuthorName { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
