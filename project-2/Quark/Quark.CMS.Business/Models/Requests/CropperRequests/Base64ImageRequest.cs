namespace Quark.CMS.Business.Models.Requests.CropperRequests
{
    public class Base64ImageRequest
    {
        public string Base64Image { get; set; }
        public string FileName { get; set; }
        public string UploadedImageType { get; set; }
    }
}
