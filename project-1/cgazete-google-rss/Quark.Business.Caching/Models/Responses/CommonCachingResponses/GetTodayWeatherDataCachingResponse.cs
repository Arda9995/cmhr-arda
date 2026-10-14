namespace Quark.Business.Caching.Models.Responses.CommonCachingResponses
{
    public class GetTodayWeatherDataCachingResponse
    {
        public string Degree { get; set; }
        public string Date { get; set; }
        public int CityID { get; set; }
        public string Status { get; set; }
        public string CityName { get; set; }
    }
}
