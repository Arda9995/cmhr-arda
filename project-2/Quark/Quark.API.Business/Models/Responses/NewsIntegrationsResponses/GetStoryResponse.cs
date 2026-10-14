using System.Text.Json.Serialization;

namespace Quark.API.Business.Models.Responses.NewsManagementResponses;

public record GetStoryResponse
{
    public bool Ok { get; set; }
    public StoryItem Data { get; set; }
}

public record StoryItem
{
    public long Id { get; set; }
    [JsonPropertyName("news_category")]
    public string NewsCategory { get; set; }
    [JsonPropertyName("news_title")]
    public string NewsTitle { get; set; }
    [JsonPropertyName("content")]
    public string Content { get; set; }
}