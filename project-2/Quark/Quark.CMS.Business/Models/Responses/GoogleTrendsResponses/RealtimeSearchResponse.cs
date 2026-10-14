namespace Quark.CMS.Business.Models.Responses.GoogleTrendsResponses;

public class RealtimeSearchResponse
{
    public List<TrendData> TrendsList { get; set; }
}

public class Image
{
    public string NewsUrl { get; set; }
    public string Source { get; set; }
    public string ImgUrl { get; set; }
}

public class Article
{
    public string ArticleTitle { get; set; }
    public string Url { get; set; }
    public string Source { get; set; }
    public string Time { get; set; }
    public string Snippet { get; set; }
}

public class TrendData
{
    public Image Image { get; set; }
    public string ShareUrl { get; set; }
    public List<Article> Articles { get; set; }
    public List<string> IdsForDedup { get; set; }
    public string Id { get; set; }
    public string Title { get; set; }
    public List<string> EntityNames { get; set; }
}