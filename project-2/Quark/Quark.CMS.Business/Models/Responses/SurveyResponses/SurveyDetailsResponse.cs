using Quark.Data.Models;

namespace Quark.CMS.Business.Models.Responses.SurveyResponses;

public class SurveyDetailsResponse
{
    public int ID { get; set; }
    public string Question { get; set; }
    public string Option1 { get; set; }
    public string Option2 { get; set; }
    public string Option3 { get; set; }
    public string Option4 { get; set; }
    public int TotalAnswers { get; set; }
    public bool IsEnabled { get; set; }
    public int Option1Answers { get; set; }
    public int Option2Answers { get; set; }
    public int Option3Answers { get; set; }
    public int Option4Answers { get; set; }
    public string CreatedDate { get; set; }
    public string LastUpdated { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? StartDate { get; set; }
    public string PhotoUrl { get; set; }
    public int CreatedBy { get; set; }
    public string Slug { get; set; }
    public SurveyItemRequest SurveyItemRequest { get; set; }

}