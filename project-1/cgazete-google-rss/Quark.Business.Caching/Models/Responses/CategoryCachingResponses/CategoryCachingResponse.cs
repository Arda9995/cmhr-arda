namespace Quark.Business.Caching.Models.Responses.CategoryCachingResponses;

public class CategoryCachingResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public int? ParentID { get; set; }
    public string Url { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string Breadcrumb { get; set; }
    public string PhotoUrl { get; set; }
    public string Map { get; set; }
}