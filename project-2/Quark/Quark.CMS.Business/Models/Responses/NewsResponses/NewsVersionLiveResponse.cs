using Quark.CMS.Business.Models.Requests.NewsRequests;

namespace Quark.CMS.Business.Models.Responses.NewsResponses;

public class NewsVersionLiveResponse
{
    public NewsVersionResponseComparable Version { get; set; }
    public NewsVersionResponseComparable News { get; set; }
    public NewsVersionResponseComparable Live { get; set; }
    public int StatusID { get; set; }
    public string VersionCreatedDate { get; set; }
    public string VersionCreatedBy { get; set; }
    public string PublishedDate { get; set; }
    public string PublishedBy { get; set; }
    public bool IsLockedByCurrentUser { get; set; }
    public bool IsLocked { get; set; }
    public string LockedBy { get; set; }
    public DateTime? LockedTime { get; set; }
    public int NewsCategoryID { get; set; }
    public int NewsTypeID { get; set; }
    public string Description { get; set; }
    public DateTime ManuelUpdatedDate { get; set; }
}


public class NewsVersionResponseComparable
{
    public long ID { get; set; }
    public string Title { get; set; }
    public string UpSubtitle { get; set; }
    public string Subtitle { get; set; }
    public string Content { get; set; }
    public string[] SecondaryCategories { get; set; }
    public string SecondaryCategoryMap { get; set; }
    public string MainCategory { get; set; }
    public int CategoryID { get; set; }
    public string Slug { get; set; }
    public string Tags { get; set; }
    public string[] TagsList { get; set; }
    public string Video { get; set; }
    public List<Photo> Photos { get; set; }
    public List<PhotoGalleryItemRequest> PhotoGalleryItems { get; set; }
    public string PressNo { get; set; }
    public string Headline { get; set; }
    public bool AllowAds { get; set; }
    public bool ShowHeadline { get; set; }
    public bool AllowToSendGoogleSideMap { get; set; }
    public bool AllowGoogleIndex { get; set; }
    public bool AllowToSendRSS { get; set; }
    public string SourceName { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string SocialMediaTitle { get; set; }
    public string SourceIDs { get; set; }
    public int? AuthorID { get; set; }
    public string AuthorName { get; set; }
    public string CaricaturistName { get; set; }
    public int? VideoPlayerID { get; set; }
    public bool IsVersion { get; set; }
    public string UpHeadline { get; set; }
    public string SubHeading { get; set; }
    public string UpSubHeading { get; set; }
    public bool ShowUpHeadline { get; set; }
    public bool ShowSubHeading { get; set; }
    public bool ShowUpSubHeading { get; set; }
    public string RedirectLink { get; set; }

}