using Quark.Data.Models.Common;

namespace Quark.Business.Caching.Models.Responses.CommonCachingResponses;

public class GetAllWeatherDataCachingResponse
{
    public int ID { get; set; }
    public List<WeatherData> WeatherDatas { get; set; }
}