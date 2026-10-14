namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses;

public class CategoryNewsCaching
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