using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.NewsPublishedPhotoGalleryItems")]
public class NewsPublishedPhotoGalleryItem
{
    public int ID { get; set; }
    public long NewsID { get; set; }
    public string Content { get; set; }
    public string PhotoUrl { get; set; }
    public int Order { get; set; }
}