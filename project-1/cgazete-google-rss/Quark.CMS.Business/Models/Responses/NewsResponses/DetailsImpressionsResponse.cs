namespace Quark.CMS.Business.Models.Responses.NewsResponses;

public class DetailsImpressionsResponse
{
    public List<string> LabelsMinute { get; set; }
    public List<int> MinData { get; set; }
    public List<string> LabelsHour { get; set; }
    public List<int> HourData { get; set; }
    public List<string> LabelsDay { get; set; }
    public List<int> DayData { get; set; }
}