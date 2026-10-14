using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.NewsPhotoGalleryItems")]
public class NewsPhotoGalleryItem
{
    public int ID { get; set; }
    public long NewsID { get; set; }
    public int Order { get; set; }
    public string PhotoUrl { get; set; }
    public string Content { get; set; }
}