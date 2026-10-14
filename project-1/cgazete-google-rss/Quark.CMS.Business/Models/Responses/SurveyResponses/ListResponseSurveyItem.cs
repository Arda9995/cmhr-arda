using Microsoft.AspNetCore.Http;

namespace Quark.CMS.Business.Models.Responses.SurveyResponses;

public class ListResponseSurveyItem
{
    public int ID { get; set; }
    public string Question { get; set; }
    public int TotalAnswers { get; set; }
    public string IsEnabledText { get; set; }
    public bool IsEnabled { get; set; }
    public string CreatedDate { get; set; }
    public int CreatedBy { get; set; }
    public string PhotoUrl { get; set; }
    public IFormFile Photo { get; set; }

}