using Newtonsoft.Json;

namespace Quark.Cloudflare.Adapter.Models;

    public class CloudflareFileInfo
    {
        [JsonProperty("files")]
        public List<string> Files { get; set; }
    }



