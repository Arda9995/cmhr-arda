namespace Quark.CMS.Business.Models.Requests.NotificationRequests;

public class SendMobilNotificationRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageURL { get; set; }
    public int NewsID { get; set; }
    public string NewsTypeName { get; set; }
}