using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.MediaGallery;

[Table("MediaGallery.ArchiveImages")]
public class ArchiveImage
{
    public int ID { get; set; }
    public int FolderID { get; set; }
    public string ImageURL { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedDate { get; set; }
}