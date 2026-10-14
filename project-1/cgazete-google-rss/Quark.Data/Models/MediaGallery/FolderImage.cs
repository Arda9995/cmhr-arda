using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.MediaGallery;

[Table("MediaGallery.FolderImages")]
public class FolderImage
{
    public int ID { get; set; }
    public int FolderID { get; set; }
    public string ImageURL { get; set; }
    public string ImageName { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedDate { get; set; }
}