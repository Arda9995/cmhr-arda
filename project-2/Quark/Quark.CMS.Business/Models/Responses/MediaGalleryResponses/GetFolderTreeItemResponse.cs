namespace Quark.CMS.Business.Models.Responses.MediaGalleryResponses;

public class GetFolderTreeItemResponse
{
    public int Id { get; set; }
    public string text { get; set; }
    public bool children { get; set; }
    public string type { get; set; }
    public string icon { get; set; }
}