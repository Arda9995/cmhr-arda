using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.NewsVersionsPhotoGalleryItems")]
public class NewsVersionsPhotoGalleryItem
{
    public long ID { get; set; }
    public long VersionID { get; set; }
    public long NewsID { get; set; }
    public string PhotoUrl { get; set; }
    public string Content { get; set; }
    public int Order { get; set; }
}