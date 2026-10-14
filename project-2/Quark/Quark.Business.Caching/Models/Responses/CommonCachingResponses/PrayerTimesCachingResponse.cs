using Quark.Data.Models.Common;

namespace Quark.Business.Caching.Models.Responses.CommonCachingResponses;

public class PrayerTimesCachingResponse
{
    public int ID { get; set; }
    public List<PrayerTimes> PrayerTimesList { get; set; }
}