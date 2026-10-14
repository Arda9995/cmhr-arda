using AvvaMobile.Core.Business;
using Quark.Business.Caching.Models.Responses.CommonCachingResponses;
using Quark.Data.Models.Common;

namespace Quark.Business.Caching.Interfaces;

public interface ICommonCachingService
{
    Task<ServiceResult> RefreshCurrencySelectListWithSlug();
    Task<ServiceResult> RefreshCurrencySelectListWithID();
    Task<ServiceResult> RefreshCurrencies();
    Task<ServiceResult> RefreshPrayerTimes();
    Task<ServiceResult<List<PrayerTimes>>> GetPrayerTimes(int cityId);
    Task<ServiceResult> RefreshCurrencyDailyData(string currency);
    Task<ServiceResult<CurrencySelectListItemCachingResponse>> GetCurrencySelectListWithSlug();
    Task<ServiceResult<CurrencySelectListItemCachingResponse>> GetCurrencySelectListWithID();
    Task<ServiceResult<List<CurrencyCachingResponse>>> GetCurrencies();
    Task<ServiceResult<List<CurrencyDailyDataCachingResponse>>> GetCurrencyDailyData(string currency);
    Task<ServiceResult<List<LocationCacheResponse>>> GetCities();
    Task<ServiceResult> RefreshCities();
    Task<ServiceResult<List<LocationCacheResponse>>> GetDistricts();
    Task<ServiceResult> RefreshDistricts();
    Task<ServiceResult<GetTodayWeatherDataCachingResponse>> GetTodayWeatherData(int cityID);
    Task<ServiceResult<List<WeatherData>>> GetAllWeatherData(int cityID);
    Task<ServiceResult> RefreshTodayWeatherData();
    Task<ServiceResult> RefreshAllWeatherData();
    Task<ServiceResult> RefreshLatestCurrencyNews();
    Task<ServiceResult<List<GetLatestCurrencyNews>>> GetLatestCurrencyNews();
}