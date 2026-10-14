namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses;

public class DbProcedureNewsDetailResponse
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
    public string SecondaryCategoryIDList { get; set; }
    public string SecondaryCategoryNameList { get; set; }
    public string Content { get; set; }
    public string CoverPhoto { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public string UpdateDateForMetaTag { get; set; }
    public DateTime PublishDate { get; set; }
    public string PublishDateForMetaTag { get; set; }
    public string SourceIDs { get; set; }
    public int? AuthorID { get; set; }
    public int CategoryID { get; set; }
    public string PressNo { get; set; }
    public int? BoxCount { get; set; }
    public string CategorySlug { get; set; }
    public string CategoryName { get; set; }
    public string CategoryBreadcrumb { get; set; }
    public string Video { get; set; }
    public int? VideoPlayerID { get; set; }
    public string SecondaryCategoryMap { get; set; }
    public string SeoDescription { get; set; }
    public string SeoTitle { get; set; }
    public string Tags { get; set; }
    public string Sources { get; set; }
    public string PhotoGalleryItemIDs { get; set; }
    public string TagIDsList { get; set; }
    public string RelatedNewsIDsList { get; set; }
}