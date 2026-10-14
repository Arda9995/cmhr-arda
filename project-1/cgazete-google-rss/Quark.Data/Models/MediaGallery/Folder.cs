using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.MediaGallery;

[Table("MediaGallery.Folders")]
public class Folder
{
    public int ID { get; set; }
    public int? ParentID { get; set; }
    public string Name { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDateTime { get; set; }
}