namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses;

public class SurveyNewsCaching
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Question { get; set; }
    public string Slug { get; set; }
    public string ImageURL { get; set; }
    public string RemainingTime { get; set; }
    public int JoinedUserCount { get; set; }
}