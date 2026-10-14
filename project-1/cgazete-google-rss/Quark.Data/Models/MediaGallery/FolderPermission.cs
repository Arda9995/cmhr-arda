using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.MediaGallery;

[Table("MediaGallery.FolderPermissions")]
public class FolderPermission
{
    public int UserID { get; set; }
    public int FolderID { get; set; }
}