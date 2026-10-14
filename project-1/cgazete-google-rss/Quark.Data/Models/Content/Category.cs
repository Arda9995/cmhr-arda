using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.Categories")]
public class Category
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
    public string BackgroundPhotoUrl { get; set; }
    public string Map { get; set; }
}

public enum CategoryEnum : int
{
    Authors = 1,
    Astrologie = 15,
    GovernmentAnnouncement = 108,
    Survey = 116,
    Currency = 98,
    PhotoGallery = 29,
    VideoGallery = 28,
    Siyaset = 2,
    Turkiye = 3,
    Dunya = 4,
    Ekonomi = 5,
    Spor = 8,
}

public enum ZodiacSignEnum : int
{
    Koc = 67,
    Boga,
    Ikizler,
    Yengec,
    Aslan,
    Basak,
    Terazi,
    Akrep,
    Yay,
    Oglak,
    Kova,
    Balik
}
