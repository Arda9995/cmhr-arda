using Newtonsoft.Json;

namespace Quark.CMS.Business.Models.Responses.ReCaptchaResponses;

public class ReCaptchaResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("score")]
    public double Score { get; set; }
}