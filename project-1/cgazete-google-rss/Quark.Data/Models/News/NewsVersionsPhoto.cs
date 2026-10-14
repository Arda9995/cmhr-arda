using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.NewsVersionsPhotos")]
public class NewsVersionsPhoto
{
    public int ID { get; set; }
    public long NewsID { get; set; }
    public long VersionID { get; set; }
    public int PhotoTypeID { get; set; }
    public string PhotoUrl { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}