namespace Quark.API.Business.Models.Responses.Notifications
{
    public class ListNotificationsResponse
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public bool IsRead { get; set; }
        public int? NotificationSenderID { get; set; }
        public string NotificationSenderName { get; set; }
        public string NotificationSenderProfilePhoto { get; set; }
        public string ElapsedTime { get; set; }
    }
}