namespace Quark.CMS.Business.Models.Responses.NewsResponses;

public class ListResponseNewsItem
{
    public long ID { get; set; }
    public long NewsID { get; set; }
    public string PhotoUrl { get; set; }
    public string Title { get; set; }
    public string CreatedByName { get; set; }
    public string Category { get; set; }
    public string CreatedDate { get; set; }
    public string UpdatedDate { get; set; }
    public string PublishedDate { get; set; }
    public string Status { get; set; }
    public int Views { get; set; }
    public string Type { get; set; }
    public bool IsLockedByCurrentUser { get; set; }
    public string UpHeadline { get; set; }
    public string SubHeading { get; set; }
    public string UpSubHeading { get; set; }
    public bool ShowUpHeadline { get; set; }
    public bool ShowSubHeading { get; set; }
    public bool ShowUpSubHeading { get; set; }
    public string RedirectLink { get; set; }
    public List<int> Impressions { get; set; }
    public List<DateTime> ImpressionDates { get; set; }
    public bool IsImpressionIncreasing { get; set; }
    public int StatusId { get; set; }
    public string Tags { get; set; }
    public int RelatedNewsCount { get; set; }
    public int TagsCount { get; set; }
    public bool HasPublishRight { get; set; }
    public bool HasUnPublishRight { get; set; }
    public bool HasCreateUpdateRight { get; set; }
    public string ManuelUpdatedDate { get; set; }
    public int LockedBy { get; set; }
    public bool IsPublished { get; set; }
}