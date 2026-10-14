
using Quark.CMS.Business.Models.Requests.NewsRequests;
using Quark.Data.Models;

namespace Quark.CMS.Business.Models.Responses.NewsResponses;

public class  DetailsNewsResponse
{
    public long NewsID { get; set; }
    public string Title { get; set; }
    public string UpSubTitle { get; set; }
    public string SubTitle { get; set; }
    public string Content { get; set; }
    public string Type { get; set; }
    public string MainCategory { get; set; }
    public string[] SecondaryCategories { get; set; }
    public string SecondaryCategoryMap { get; set; }
    public string Tags { get; set; }
    public string[] TagsList { get; set; }
    public int StatusID { get; set; }
    public List<Photo> Photos { get; set; }
    public bool IsLocked { get; set; }
    public string LockedBy { get; set; }
    public bool IsLockedByCurrentUser { get; set; }
    public string ScheduledDate { get; set; }
    public List<Version> Versions { get; set; }
    public string VersionCreatedDate { get; set; }
    public string VersionCreatedBy { get; set; }
    public string UpdateDate { get; set; }
    public string UpdatedBy { get; set; }
    public string PublishDate { get; set; }
    public string PublishEndDate { get; set; }
    public long VersionID { get; set; }
    public int ViewCount { get; set; }
    public string Slug { get; set; }
    public string RedirectURL { get; set; }
    public DateTime? LockedTime { get; set; }
    public string Video { get; set; }
    public bool IsPublished { get; set; }
    public int NewsTypeID { get; set; }
    public List<PhotoGalleryItemRequest> PhotoGalleryItems { get; set; }
    public string PressNo { get; set; }
    public string Headline { get; set; }
    public bool AllowAds { get; set; }
    public Rights HasRights { get; set; }
    public int CreatedBy { get; set; }
    public string SourceName { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public SocialMediaRequest SocialMedias { get; set; }
    public string SourceIDs { get; set; }
    public int? AuthorID { get; set; }
    public string AuthorName { get; set; }
    public string CaricaturistName { get; set; }
    public int? VideoPlayerID { get; set; }
    public string UpHeadline { get; set; }
    public string SubHeading { get; set; }
    public string UpSubHeading { get; set; }
    public bool ShowHeadline { get; set; }
    public bool ShowUpHeadline { get; set; }
    public bool ShowSubHeading { get; set; }
    public bool ShowUpSubHeading { get; set; }
    public string RedirectLink { get; set; }
    public SurveyItemRequest SurveyItemRequest { get; set; }
    public string ManuelUpdatedDate { get; set; }
    public string StatusName { get; set; }
    public int LockedByID { get; set; }
}


public class Photo
{
    public string PhotoURL { get; set; }
    public int PhotoTypeID { get; set; }
    public string PhotoTypeName { get; set; }
    public string Description { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    
}

public class Version
{
    public long VersionID { get; set; }
    public string UserPhotoURL { get; set; }
    public string UserName { get; set; }
    public string CreatedDate { get; set; }
    
}

public class Rights
{
    public bool Unlock { get; set; }
    public bool Publish { get; set; }
    public bool UnPublish { get; set; }
    public bool NewsCreateUpdate { get; set; }
}