namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses;

public class GetNewsListCachingResponse
{
    public int ID { get; set; }
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

    public List<CategoryNewsCaching> CategoryTwo { get; set; } = new List<CategoryNewsCaching>();
    public List<CategoryNewsCaching> CategorySix { get; set; } = new List<CategoryNewsCaching>();
    public List<CategoryNewsCaching> CategoryNine { get; set; } = new List<CategoryNewsCaching>();
    public List<CategoryNewsCaching> CategoryTen { get; set; } = new List<CategoryNewsCaching>();
    public List<CategoryNewsCaching> ZodiacSignNews { get; set; } = new List<CategoryNewsCaching>();
    public List<CategoryNewsCaching> CategoryOthers { get; set; } = new List<CategoryNewsCaching>();
    public List<CategoryNewsCaching> MobileCategoryNews { get; set; } = new List<CategoryNewsCaching>();
    public List<SurveyNewsCaching> SurveyNewsTwo { get; set; } = new List<SurveyNewsCaching>();
    public List<SurveyNewsCaching> SurveyNewsSix { get; set; } = new List<SurveyNewsCaching>();
    public List<SurveyNewsCaching> SurveyNewsOthers { get; set; } = new List<SurveyNewsCaching>();

    public List<ZodiacSignNewsForSliderCaching> ZodiacSignNewsForSlider { get; set; } =
        new List<ZodiacSignNewsForSliderCaching>();

    public List<CategoryNewsCaching> GovernmentAnnouncementNews { get; set; } = new List<CategoryNewsCaching>();
}