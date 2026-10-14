namespace Quark.CMS.Business.Models.Requests.NotificationRequests
{
    public class SendWebNotificationRequest
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string WpChromeImage { get; set; }
    }
}
