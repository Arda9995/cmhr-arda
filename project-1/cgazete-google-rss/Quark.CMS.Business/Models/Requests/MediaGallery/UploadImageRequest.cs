using Microsoft.AspNetCore.Http;

namespace Quark.CMS.Business.Models.Requests.MediaGallery;

public class UploadImageRequest
{
    public int FolderId { get; set; }
    public IFormFile Image { get; set; }
}