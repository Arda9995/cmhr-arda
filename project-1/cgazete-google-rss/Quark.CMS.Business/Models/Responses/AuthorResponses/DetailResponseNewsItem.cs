namespace Quark.CMS.Business.Models.Responses.AuthorResponses;

public class DetailResponseNewsItem
{
    public long ID { get; set; }
    public string PhotoUrl { get; set; }
    public string Title { get; set; }
    public string TitleTooltip { get; set; }
    public string Category { get; set; }
    public string CreatedDate { get; set; }
    public string UpdatedDate { get; set; }
    public string PublishedDate { get; set; }
    public string Status { get; set; }
    public string ViewCount { get; set; }
    public string Type { get; set; }
    public bool IsLockedAuthorized { get; set; }
    
    public List<int> Impressions { get; set; }
    public List<DateTime> ImpressionDates { get; set; }
    public bool IsImpressionIncreasing { get; set; }
    public string Tags { get; set; }
    public int RelatedNewsCount { get; set; }
    public int TagsCount { get; set; }
    public int StatusId { get; set; }
}