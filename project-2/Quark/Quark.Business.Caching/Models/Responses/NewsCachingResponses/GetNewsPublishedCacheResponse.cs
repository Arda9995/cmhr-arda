namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses;

public class GetNewsPublishedCacheResponse
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Headline { get; set; }
    public string SubTitle { get; set; }
    public string UpSubtitle { get; set; }
    public string CoverPhoto { get; set; }
    public string Slug { get; set; }
    public DateTime PublishDate { get; set; }
    public int NewsTypeID { get; set; }
    public int? AuthorID { get; set; }
    public int? CategoryID { get; set; }
    public string SoruceIDs { get; set; }
}