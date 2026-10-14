using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.NewsPhotos")]
public class NewsPhoto
{
    public int ID { get; set; }
    public long NewsID { get; set; }
    public int PhotoTypeID { get; set; }
    public string PhotoUrl { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}