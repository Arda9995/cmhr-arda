using System.Text.Json.Serialization;

namespace Quark.API.Business.Models.Responses.NewsManagementResponses;

public record GetListStoriesResponse
{
    public bool Ok { get; set; }
    public List<ListStoryItem> Data { get; set; }
    
}

public record ListStoryItem
{
    public long Id { get; set; }
    [JsonPropertyName("news_category")]
    public string NewsCategory { get; set; }
    [JsonPropertyName("news_title")]
    public string NewsTitle { get; set; }
}

