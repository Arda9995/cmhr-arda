namespace Quark.API.Business.Models.Responses.NewsResponses
{
    public class GetNewsListResponse
    {
        public string SeoDescription { get; set; }
        public string SeoTitle { get; set; }
        public int CategoryID { get; set; }
        public string CategorySlug { get; set; }
        public string CategoryBreadcrumb { get; set; }
        public string CategoryName { get; set; }

        public int SourceID { get; set; }
        public string SourceSlug { get; set; }
        public string SourceSlugWithoutClientURL { get; set; }
        public string SourceBreadcrumb { get; set; }
        public string SourceName { get; set; }

        public int TagID { get; set; }
        public string TagSlug { get; set; }
        public string TagSlugWithoutClientURL { get; set; }
        public string TagBreadcrumb { get; set; }
        public string TagName { get; set; }

        public List<CategoryNews> CategoryTwo { get; set; } = new List<CategoryNews>();
        public List<CategoryNews> CategorySix { get; set; } = new List<CategoryNews>();
        public List<CategoryNews> CategoryNine { get; set; } = new List<CategoryNews>();
        public List<CategoryNews> CategoryTen { get; set; } = new List<CategoryNews>();
        public List<CategoryNews> ZodiacSignNews { get; set; } = new List<CategoryNews>();
        public List<CategoryNews> CategoryOthers { get; set; } = new List<CategoryNews>();
        public List<CategoryNews> MobileCategoryNews { get; set; } = new List<CategoryNews>();
        public List<SurveyNews> SurveyNewsTwo{ get; set; } = new List<SurveyNews>();
        public List<SurveyNews> SurveyNewsSix{ get; set; } = new List<SurveyNews>();
        public List<SurveyNews> SurveyNewsOthers{ get; set; } = new List<SurveyNews>();
        public List<ZodiacSignNewsForSlider> ZodiacSignNewsForSlider { get; set; } = new List<ZodiacSignNewsForSlider>();
        public List<CategoryNews> GovernmentAnnouncementNews { get; set; } = new List<CategoryNews>();
       
    }
    public class ZodiacSignNewsForSlider
    {
        public string Slug { get; set; }
        public string Headline { get; set; }
        public string UpHeadline { get; set; }
        public bool ShowUpHeadline { get; set; }
        public string ImageURL { get; set; }
    }


}
