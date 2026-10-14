using System.Text.Json.Serialization;

namespace Quark.Notification.Adapter.Models.Responses
{
    public class NetmeraNotificationResponse
    {
        [JsonPropertyName("notificationKey")]
        public int NotificationKey { get; set; }
    }
}