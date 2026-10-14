namespace Quark.CMS.Business.Models.Responses.MediaGalleryResponses;

public class GetFolderImagesResponse
{
    public int FolderID { get; set; }
    public List<FolderImageInfoResponse> ImageInfo { get; set; }
}