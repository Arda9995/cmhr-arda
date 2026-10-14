namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses;

public class CacheTodaysColumnArticles
{
    public int NewsID { get; set; }
    public string Headline { get; set; }
    public string UpSubtitle { get; set; }
    public string Slug { get; set; }
    public int AuthorID { get; set; }
    public string AuthorPhoto { get; set; }
    public string AuthorFullName { get; set; }
}