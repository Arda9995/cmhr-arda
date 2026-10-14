namespace Quark.CMS.Business.Models.Requests.NewsRequests
{
    public class PhotoGalleryItemsSaveRequest
    {
        public List<PhotoGalleryItems> Items { get; set; }
        public int NewsID { get; set; }
    }

    public class PhotoGalleryItems
    {
        public string Content { get; set; }
        public string ImageURL { get; set; }
        public int Order { get; set; }
    }
}
