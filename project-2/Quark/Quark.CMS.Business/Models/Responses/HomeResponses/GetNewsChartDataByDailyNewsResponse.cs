namespace Quark.CMS.Business.Models.Responses.HomeResponses
{
    public class GetNewsChartDataByDailyNewsResponse
    {
        public List<string> NewsCreateDates { get; set; }
        public List<int> NewsCounts { get; set; }
    }
}
