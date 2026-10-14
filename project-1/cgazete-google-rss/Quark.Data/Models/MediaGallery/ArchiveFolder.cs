using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.MediaGallery;

[Table("MediaGallery.ArchiveFolders")]
public class ArchiveFolder
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int? ParentID { get; set; }
}