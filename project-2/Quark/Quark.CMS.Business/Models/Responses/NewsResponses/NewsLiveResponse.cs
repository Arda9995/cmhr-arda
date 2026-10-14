using Quark.CMS.Business.Models.Requests.NewsRequests;

namespace Quark.CMS.Business.Models.Responses.NewsResponses;

public class NewsLiveResponse
{
    public NewsLiveResponseComparable Live { get; set; }
    public NewsLiveResponseComparable News { get; set; }
    public int StatusID { get; set; }
    public string VersionCreatedDate { get; set; }
    public string VersionCreatedBy { get; set; }
    public bool IsLockedByCurrentUser { get; set; }
    public bool IsLocked { get; set; }
    public string LockedBy { get; set; }
    public DateTime? LockedTime { get; set; }
    public int NewsCategoryID { get; set; }
    public int NewsTypeID { get; set; }

}

public class NewsLiveResponseComparable
{
    public long ID { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Content { get; set; }
    public string[] Categories { get; set; }
    public string Slug { get; set; }
    public string Tags { get; set; }
    public string[] TagsList { get; set; }
    public string Video { get; set; }
    public List<Photo> Photos { get; set; }
    public List<PhotoGalleryItemRequest> PhotoGalleryItems { get; set; }
}