using Quark.Data.Models;

namespace Quark.Business.Caching.Models.Responses.PageCachingResponses
{
	public class PageNewsCachingResponse
	{
		public string PageKey { get; set; }
		public int BlockID { get; set; }
		public string BlockKey { get; set; }
		public string BlockTitle { get; set; }
		public string BlockName { get; set; }
		public string CategoryPhotoUrl { get; set; }
		public int NewsID { get; set; }
		public string NewsContent { get; set; }
		public string NewsTitle { get; set; }
		public int NewsTypeID { get; set; }
		public string NewsSubtitle { get; set; }
		public string NewsUpSubtitle { get; set; }
		public string NewsHeadline { get; set; }
		public string NewsUpHeadline { get; set; }
        public bool ShowUpHeadline { get; set; }
        public bool ShowHeadline { get; set; }
        public bool ShowUpSubHeading { get; set; }
        public string NewsUpSubHeading { get; set; }
		public string NewsSubHeading { get; set; }
		public string CategoryName { get; set; }
		public string NewsPhoto { get; set; }
        public string NewsSlug { get; set; }
        public int? BlockCategoryID { get; set; }
        public string BlockCategoryName { get; set; }
        public string BlockCategorySlug { get; set; }
        public string BlockCategoryBackgroundPhotoUrl { get; set; }
		public SurveyItem SurveyItem { get; set; }
    }

	public class SurveyItem
	{
        public string Question  { get; set; }
        public int? JoinedUserCount { get; set; }
    }
}
