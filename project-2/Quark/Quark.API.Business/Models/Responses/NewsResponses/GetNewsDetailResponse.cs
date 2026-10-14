using DocumentFormat.OpenXml.Office2010.Ink;
using Newtonsoft.Json;

namespace Quark.API.Business.Models.Responses.NewsResponses;

public class GetNewsDetailResponse
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
    public List<RelatedNews> RelatedNewsList { get; set; } = new List<RelatedNews>();
    public List<CategoryNews> CategoryNewsList { get; set; } = new List<CategoryNews>();
    public List<PhotoGalleryItem> PhotoGalleryItems { get; set; } = new List<PhotoGalleryItem>();
    public List<TagList> TagList { get; set; } = new List<TagList>();
    public string Tags { get; set; }
    public List<SourceList> SourceList { get; set; } = new List<SourceList>();
    public string Sources { get; set; }
    public ColumnArticleDetails ColumnArticleDetails { get; set; } = new ColumnArticleDetails();
    public SurveyElements SurveyElements { get; set; }
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

public class SourceList
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
}

public class PhotoGalleryItem
{
    public string ImageURL { get; set; }
    public string Content { get; set; }
}
public class TagList
{
    public int ID { get; set; }
    public string Text { get; set; }
    public string Slug { get; set; }
}

public class RelatedNews
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Slug { get; set; }
    public string ImageURL { get; set; }
}

public class ColumnArticleDetails
{
    public string AuthorPhoto { get; set; }
    public string AuthorFullName { get; set; }
    public string AuthorSlug { get; set; }
    public string AuthorSocialX { get; set; }
    public string AuthorSocialInstagram { get; set; }
    public string AuthorSocialFacebook { get; set; }
    public string AuthorSocialLinkedIn { get; set; }
    public string AuthorSocialMail { get; set; }
    public List<AuthorsLastColumnArticles> AuthorsLastColumnArticles { get; set; } = new List<AuthorsLastColumnArticles>();
    public List<TodaysColumnArticles> AuthorsTodaysColumnArticles { get; set; } = new List<TodaysColumnArticles>();
}

public class TodaysColumnArticles
{
    public int NewsID { get; set; }
    public string Headline { get; set; }
    public string UpSubtitle { get; set; }
    public string Slug { get; set; }
    public int AuthorID { get; set; }
    public string AuthorPhoto { get; set; }
    public string AuthorFullName { get; set; }
}

public class AuthorsLastColumnArticles
{
    public int NewsID { get; set; }
    public string Headline { get; set; }
    public string UpSubtitle { get; set; }
    public string Slug { get; set; }
    public string PublishDate { get; set; }
}


public class CategoryNews
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

public class SurveyNews
{
	public int ID { get; set; }
	public string Title { get; set; }
	public string Question { get; set; }
	public string Slug { get; set; }
	public string ImageURL { get; set; }
    public string RemainingTime { get; set; }
    public int JoinedUserCount { get; set; }

}

public class SurveyElements
{
    public string SurveyQuestion { get; set; }
    public int JoinedUserCount { get; set; }
    public string RemainingTime { get; set; }
	public List<SurveySection> SurveySections { get; set; }
    public List<CategoryNews> CategoryNews { get; set; }
    public List<SurveyNews> SurveyNewsThird { get; set; }
    public List<SurveyNews> SurveyNewsQuad { get; set; }
}

public class SurveySection
{
    public int OptionID { get; set; }
    public int SurveyID { get; set; }
    public string Option { get; set; }
    public int? Percent { get; set; }
}