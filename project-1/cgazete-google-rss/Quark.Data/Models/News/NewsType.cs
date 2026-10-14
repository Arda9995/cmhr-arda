using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.NewsTypes")]
public class NewsType
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public string Icon { get; set; }
}
public enum NewsTypesEnum : int
{
    News = 1,
    PhotoGallery = 2,
    VideoGallery = 3,
    Ad = 4,
    ColumnArticles = 5,
    Advertisement = 4,
    CaricaturistNews = 6,
    ZodiacSigns = 7,
    GovernmentAnnouncement = 8,
    Survey = 10,
}