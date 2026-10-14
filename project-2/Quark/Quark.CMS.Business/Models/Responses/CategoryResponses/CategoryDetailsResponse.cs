namespace Quark.CMS.Business.Models.Responses.CategoryResponses;

public class CategoryDetailsResponse
{
    public int ID { get; set; }
    public string CategoryName { get; set; }
    public string SeoDescription { get; set; }
    public string SeoTitle { get; set; }
    public string Url { get; set; }
    public string Breadcrumb { get; set; }
    public string PhotoUrl { get; set; }
    public string BackgroundPhotoUrl { get; set; }
}