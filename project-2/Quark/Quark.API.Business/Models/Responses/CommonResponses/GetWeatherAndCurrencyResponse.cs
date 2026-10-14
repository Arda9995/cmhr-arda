namespace Quark.API.Business.Models.Responses.CommonResponses
{
    public class GetWeatherAndCurrencyResponse
    {
        public GetTodayCurrencyDataResponse Currency { get; set; }
        public GetTodayWeatherDataResponse Weather { get; set; }
    }
}
