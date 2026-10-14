namespace Quark.Business.Caching.Models.Responses.PromptCachingResponses;

public class PromptCachingResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }
    public int PromptTypeID { get; set; }
    public string RequestType { get; set; }
}