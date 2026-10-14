using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.MediaGallery;

[Table("MediaGallery.Favourites")]
public class Favourite
{
    public int ID { get; set; }
    public int UserID { get; set; }
    public int ImageID { get; set; }
}