namespace Quark.CMS.Business.Models.Requests.CropperRequests;

public class EditRequest
{
    public string ImgSrc { get; set; }
    public int ImgId { get; set; }
    public bool IsNotEditor { get; set; }
}