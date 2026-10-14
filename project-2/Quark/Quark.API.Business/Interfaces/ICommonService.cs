using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.API.Business.Models.Requests.CommonRequest;
using Quark.API.Business.Models.Responses.CommonResponses;

namespace Quark.API.Business.Interfaces;

public interface ICommonService
{
    ServiceResult<List<SelectListItem>> GetDefaultSelectItems();

    Task<ServiceResult<List<SelectListItem>>> GetGenders();
	Task<ServiceResult<List<GetCurrencyResponse>>> GetCurrencies();
	Task<ServiceResult<List<string>>> GetCityNames();
	Task<ServiceResult<GetCurrencyPageResponse>> GetCurrencyPage(int count, string currency);
	Task<ServiceResult> TestElasticAsync();
    Task<ServiceResult<WeatherPageResponse>> GetWeatherData();
    Task<ServiceResult<WeatherDetailPageResponse>> GetWeatherDetail(string city);
    Task<ServiceResult<GetPrayerTimesResponse>> GetPrayerTimes(string city);
    Task<ServiceResult<GetReadListPageResponse>> GetReadList();
    Task<ServiceResult<ReadListItemsResponse>> GetReadListItem(string readList, int page, int pageSize);
    Task<ServiceResult<GetTodayWeatherDataResponse>> GetTodayWeatherData();
    Task<ServiceResult<GetTodayCurrencyDataResponse>> GetTodayCurrencyData();
    Task<ServiceResult<string>> CreateSetrowSubscription(CreateWebSetrowSubscriptionRequest subscriptionRequest);
}