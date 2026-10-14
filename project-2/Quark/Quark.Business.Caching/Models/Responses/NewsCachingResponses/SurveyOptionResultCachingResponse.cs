namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses;

public class SurveyOptionResultCachingResponse
{
    public int SurveyID { get; set; }
    public List<SurveyOptionsResult> SurveyOptions { get; set; } = new List<SurveyOptionsResult>();
}


public class SurveyOptionsResult
{
    public int OptionID { get; set; }
    public string OptionName { get; set; }
    public int PercentRatio { get; set; }
    public long AnswerCount { get; set; }
}