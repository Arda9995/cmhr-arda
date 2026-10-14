namespace Quark.CMS.Business.Models.Requests.MediaGallery;

public class AddFolderRequest
{
    public int ParentId { get; set; }
    public string Name { get; set; }
}