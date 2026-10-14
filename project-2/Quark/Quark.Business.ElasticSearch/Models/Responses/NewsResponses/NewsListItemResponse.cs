namespace Quark.Business.ElasticSearch.Models.Responses.NewsResponses;

public class NewsListItemResponse
{
    public long ID { get; set; }
    public long NewsID { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Headline { get; set; }
    public string Content { get; set; }
    public string Slug { get; set; }
    public string PressNo { get; set; }
    public bool AllowAds { get; set; }
    public string Video { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string SourceIDs { get; set; }
    public string SourceName { get; set; }
    public List<int> TagIDs { get; set; }
    public string Tags { get; set; }
    public int? AuthorID { get; set; }
    public string AuthorName { get; set; }
    public int CaricaturistID { get; set; }
    public string CaricaturistName { get; set; }
    public int CategoryID { get; set; }
    public string CategoryName { get; set; }
    public string CategoryMap { get; set; }
    public string SecondaryCategories { get; set; }
    public string SecondaryCategoryMap { get; set; }
    public DateTime PublishedDate { get; set; }
    public int PublishedByID { get; set; }
    public string PublishedByName { get; set; }
    public int NewsTypeID { get; set; }
    public List<Photo> Photos { get; set; }
    public List<PhotoGalleryItem> PhotoGalleryItems { get; set; }
}

public class Photo
{
    public int TypeID { get; set; }
    public string TypeName { get; set; }
    public string TypeKey { get; set; }
    public string Url { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class PhotoGalleryItem
{
    public long ID { get; set; }
    public string Subtitle { get; set; }
    public string Content { get; set; }
    public string Url { get; set; }
    public int Order { get; set; }
}