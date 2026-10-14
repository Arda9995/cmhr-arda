namespace Quark.CMS.Business.Models.Responses.HomeResponses;

public class HomeResponse
{
    public int TotalPublishedNewsCount { get; set; }
    public int DailyPublishedNewsCount { get; set; }
    public int DailyTotalViewsCount { get; set; }
    public int CurrentViewerCount { get; set; }
    public int DailyCreatedNewsCount { get; set; }
    public int NewsWithTagsCount { get; set; }
    public int NewsWithRelatedNewsCount { get; set; }
    public List<SystemLog> SystemLogs { get; set; }
}

public class SystemLog
{
    public int ID { get; set; }
    public string UserName { get; set; }
    public string Message { get; set; }
    public string CreatedDate { get; set; }
}