using System.Text.Json.Serialization;

namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses;

public class GetCachingNewsDetailResponse
{
    public int ID { get; set; }
    public string NewsSlug { get; set; }
    public int NewsTypeID { get; set; }
    public string Title { get; set; }
    public string UpHeadline { get; set; }
    public string Headline { get; set; }
    public string SubTitle { get; set; }
	public string UpSubtitle { get; set; }
	public string Subheading { get; set; }
	public string UpSubheading { get; set; }
	public List<int> SecondaryCategoryIDList { get; set; }
	public List<string> SecondaryCategoryNameList { get; set; }
	public string Content { get; set; }
    public string CoverPhoto { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string UpdateDateForMetaTag { get; set; }
    public DateTime PublishDate { get; set; }
    public string PublishDateForMetaTag { get; set; }
    public List<CachingNewsDetailRelatedNews> RelatedNewsList { get; set; } = new List<CachingNewsDetailRelatedNews>();
    public List<CachingNewsDetailCategoryNews> CategoryNewsList { get; set; } = new List<CachingNewsDetailCategoryNews>();
    public List<CachingNewsDetailPhotoGalleryItem> PhotoGalleryItems { get; set; } = new List<CachingNewsDetailPhotoGalleryItem>();
    public List<CachingNewsDetailTagList> TagList { get; set; } = new List<CachingNewsDetailTagList>();
    public string Tags { get; set; }
    public List<CachingNewsDetailSourceList> SourceList { get; set; } = new List<CachingNewsDetailSourceList>();
    public string Sources { get; set; }
    public CachingNewsDetailColumnArticleDetails CachingNewsDetailColumnArticleDetails { get; set; } = new CachingNewsDetailColumnArticleDetails();
    public CachingNewsDetailSurveyElements CachingNewsDetailSurveyElements { get; set; }
    public string SourceIDs { get; set; }
    public int? AuthorID { get; set; }
    public string AuthorName { get; set; }
    public string AuthorSlug { get; set; }
    public string CategoryName { get; set; }
    public string CategoryBreadcrumb { get; set; }
    public string CategorySlug { get; set; }
    public string BlockQuote { get; set; }
    public int CategoryID { get; set; }
    public string PressNo { get; set; }
    public int? BoxCount { get; set; }
    public int? VideoPlayerID { get; set; }
    public string Video { get; set; }
    [JsonIgnore]
    public string ScrollNewsIDs { get; set; }
    public List<int> NewsDetailNextNewsIDs { get; set; }
    public string SecondaryCategoryMap { get; set; }
    public string SeoDescription { get; set; }
    public string SeoTitle { get; set; }
}

public class CachingNewsDetailSourceList
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}

public class CachingNewsDetailPhotoGalleryItem
{
    public string ImageURL { get; set; }
    public string Content { get; set; }
}
public class CachingNewsDetailTagList
{
    public int ID { get; set; }
    public string Text { get; set; }
    public string Slug { get; set; }
}

public class CachingNewsDetailRelatedNews
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Slug { get; set; }
    public string ImageURL { get; set; }
}

public class CachingNewsDetailColumnArticleDetails
{
    public string AuthorPhoto { get; set; }
    public string AuthorFullName { get; set; }
    public string AuthorSlug { get; set; }
    public string AuthorSocialX { get; set; }
    public string AuthorSocialInstagram { get; set; }
    public string AuthorSocialFacebook { get; set; }
    public string AuthorSocialLinkedIn { get; set; }
    public string AuthorSocialMail { get; set; }
}

public class CachingNewsDetailTodaysColumnArticles
{
    public int NewsID { get; set; }
    public string Headline { get; set; }
    public string UpSubtitle { get; set; }
    public string Slug { get; set; }
    public int AuthorID { get; set; }
    public string AuthorPhoto { get; set; }
    public string AuthorFullName { get; set; }
}

public class CachingNewsDetailAuthorsLastColumnArticles
{
    public int NewsID { get; set; }
    public string Headline { get; set; }
    public string UpSubtitle { get; set; }
    public string Slug { get; set; }
    public string PublishDate { get; set; }
}


public class CachingNewsDetailCategoryNews
{
    public int ID { get; set; }
    public string Headline { get; set; }
    public string UpHeadline { get; set; }
    public string Slug { get; set; }
    public string SlugWithOutClientURL { get; set; }
    public string UpSubtitle { get; set; }
    public string ImageURL { get; set; }
    public string PublishDate{ get; set; }
    public string PressNo { get; set; }
}

public class CachingNewsDetailSurveyNews
{
	public int ID { get; set; }
	public string Title { get; set; }
	public string Question { get; set; }
	public string Slug { get; set; }
	public string ImageURL { get; set; }
    public string RemainingTime { get; set; }
    public int JoinedUserCount { get; set; }

}

public class CachingNewsDetailSurveyElements
{
    public string SurveyQuestion { get; set; }
    public int JoinedUserCount { get; set; }
    public string RemainingTime { get; set; }
	public List<CachingNewsDetailSurveySection> SurveySections { get; set; }
    public List<CachingNewsDetailCategoryNews> CategoryNews { get; set; }
    public List<CachingNewsDetailSurveyNews> SurveyNewsThird { get; set; }
    public List<CachingNewsDetailSurveyNews> SurveyNewsQuad { get; set; }
}

public class CachingNewsDetailSurveySection
{
    public int OptionID { get; set; }
    public int SurveyID { get; set; }
    public string Option { get; set; }
    public int? Percent { get; set; }
}