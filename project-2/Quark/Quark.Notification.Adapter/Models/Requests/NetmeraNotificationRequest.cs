using System.Text.Json.Serialization;

namespace Quark.Notification.Adapter.Models.Requests
{
    public class NetmeraNotificationRequest
    {
        [JsonPropertyName("message")]
        public NetmeraMessage Message { get; set; }

        [JsonPropertyName("target")]
        public NetmeraTarget Target { get; set; }
    }

    public class NetmeraMessage
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("platforms")]
        public List<string> Platforms { get; set; }

        [JsonPropertyName("click")]
        public NetmeraClick Click { get; set; }

        [JsonPropertyName("wpChromeImage")]
        public string WpChromeImage { get; set; }

        [JsonPropertyName("interactionRequired")]
        public bool? InteractionRequired { get; set; }

        [JsonPropertyName("iconUrl")]
        public string IconUrl { get; set; }
    }

    public class NetmeraTarget
    {
        [JsonPropertyName("sendToAll")]
        public bool SendToAll { get; set; }

    }

    public class NetmeraClick
    {

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
} 