using Newtonsoft.Json;

namespace Quark.Cloudflare.Adapter.Models
{
    public class CloudflareResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("errors")]
        public List<object> Errors { get; set; }

        [JsonProperty("messages")]
        public List<object> Messages { get; set; }
    }
}
