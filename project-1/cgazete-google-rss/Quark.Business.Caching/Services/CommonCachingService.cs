using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.CommonCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Business.ElasticSearch.Abstractions;
using Quark.Business.ElasticSearch.Constants;
using Quark.Core.Extentions;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.Common;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class CommonCachingService : BaseService, ICommonCachingService
{

    private readonly IElasticSearchService _elasticSearchService;

    public CommonCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IElasticSearchService elasticSearchService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _elasticSearchService = elasticSearchService;
    }

    public async Task<ServiceResult> RefreshCities()
    {
        var result = new ServiceResult();
        var cities = await _context.Cities
               .Select(x => new LocationCacheResponse
               {
                   Id = x.ID,
                   Name = x.Name,
                   CountryID = x.CountryID,
                   Order = x.Order,
                   CityID = x.ID,
               })
               .ToListAsync();

        await _cache.Set(RedisKeys.Cities, cities);
        return result;
    }
    public async Task<ServiceResult<List<LocationCacheResponse>>> GetCities()
    {
        var result = new ServiceResult<List<LocationCacheResponse>>();
        var cities = await _cache.Get<List<LocationCacheResponse>>(RedisKeys.Cities);
        if (cities == null)
        {
            await RefreshCities();
            result.Data = await _cache.Get<List<LocationCacheResponse>>(RedisKeys.Cities);
            return result;
        }

        result.Data = cities;
        return result;
    }
    public async Task<ServiceResult> RefreshDistricts()
    {
        var result = new ServiceResult();
        var districts = await _context.Districts
            .Select(x => new LocationCacheResponse
            {
                Id = x.ID,
                Name = x.Name,
                CityID = x.CityID,
            })
            .ToListAsync();

        await _cache.Set(RedisKeys.Districts, districts);
        return result;
    }
    public async Task<ServiceResult<List<LocationCacheResponse>>> GetDistricts()
    {
        var result = new ServiceResult<List<LocationCacheResponse>>();

        result.Data = await _cache.Get<List<LocationCacheResponse>>(RedisKeys.Districts);
        if (result.Data == null)
        {
            await RefreshDistricts();
            result.Data = await _cache.Get<List<LocationCacheResponse>>(RedisKeys.Districts);
        }

        return result;
    }
    public async Task<ServiceResult> RefreshCurrencySelectListWithSlug()
    {
        var result = new ServiceResult();

        var currencies = await (from ct in _context.Currencies.AsNoTracking()
                                orderby ct.ID
                                select new
                                {
                                    DisplayName = ct.DisplayName,
                                    Slug = ct.Slug,
                                    ID = ct.ID,
                                }).ToListAsync();

        var currenciesSelectListItem = currencies.Select(x => new SelectListItem
        {
            Text = x.DisplayName,
            Value = x.Slug,
        }).ToList();

        await _cache.Set(RedisKeys.CurrencySelectListWithSlug, currenciesSelectListItem);
        return result;
    }
    public async Task<ServiceResult> RefreshCurrencySelectListWithID()
    {
        var result = new ServiceResult();

        var currencies = await (from ct in _context.Currencies.AsNoTracking()
                                orderby ct.ID
                                select new
                                {
                                    DisplayName = ct.DisplayName,
                                    Slug = ct.Slug,
                                    ID = ct.ID,
                                }).ToListAsync();

        var currenciesSelectListItem = currencies.Select(x => new SelectListItem
        {
            Text = x.DisplayName,
            Value = x.ID.ToString(),
        }).ToList();

        await _cache.Set(RedisKeys.CurrencySelectListWithID, currenciesSelectListItem);
        return result;
    }
    public async Task<ServiceResult> RefreshCurrencies()
    {
        var result = new ServiceResult();

        var currencies = await (from ct in _context.Currencies.AsNoTracking()
                                orderby ct.ID
                                select new
                                {
                                    DisplayName = ct.DisplayName,
                                    Code = ct.Code,
                                    Slug = ct.Slug,
                                    ID = ct.ID,
                                    Name = ct.Name
                                }).ToListAsync();

        await _cache.Set(RedisKeys.GetCurrencyData, currencies);
        return result;
    }
    public async Task<ServiceResult> RefreshPrayerTimes()
    {
        var result = new ServiceResult();

        var prayerTimes = await _context.PrayerTimes.AsNoTracking().ToListAsync();
        
        if (!prayerTimes.Any())
        {
            return result;
        }
        
        var cities = await _context.Cities.AsNoTracking().ToListAsync();

        foreach (var city in cities)
        {
            var prayerTime = prayerTimes.Where(x => x.CityID == city.ID).ToList();
            var cacheModel = new PrayerTimesCachingResponse
            {
                ID = city.ID,
                PrayerTimesList = prayerTime
            };
            
            await _elasticSearchService.UpdateDocumentAsync(cacheModel,city.ID.ToString(),IndexNames.PrayerTimes);
        }
        
        await _cache.Set(RedisKeys.PrayerTimes, prayerTimes);
        return result;
    }
    public async Task<ServiceResult<List<PrayerTimes>>> GetPrayerTimes(int cityId)
    {
        var result = new ServiceResult<List<PrayerTimes>>();
        var cacheData = await _elasticSearchService.GetDocumentAsync<PrayerTimesCachingResponse>(cityId.ToString(), IndexNames.PrayerTimes);
        if (cacheData == null)
        {
            await RefreshPrayerTimes();
            cacheData = await _elasticSearchService.GetDocumentAsync<PrayerTimesCachingResponse>(cityId.ToString(), IndexNames.PrayerTimes);
        }
        
        if (cacheData == null)
        {
            var redisCacheData = await _cache.Get<List<PrayerTimes>>(RedisKeys.PrayerTimes);
            if (redisCacheData == null)
            {
                result.SetError("Prayer times not found");
                return result;
            }
            result.Data = redisCacheData.Where(x => x.CityID == cityId).ToList();
            return result;
        }

        result.Data = cacheData.PrayerTimesList;
        return result;
    }
    public async Task<ServiceResult> RefreshCurrencyDailyData(string currency)
    {
        var result = new ServiceResult();

        var currencyData = await (from ct in _context.Currencies.AsNoTracking()
                                  join cv in _context.CurrencyDailyValues on ct.ID equals cv.CurrencyID
                                  where ct.Slug == currency
                                  orderby cv.Date descending
                                  select new CurrencyDailyDataCachingResponse
                                  {
                                      ID = ct.ID,
                                      Slug = ct.Slug,
                                      Name = ct.Name,
                                      BuyingValue = cv.BuyingValue,
                                      SellingValue = cv.SellingValue,
                                      Date = cv.Date,
                                      DisplayName = ct.DisplayName,
                                      StateID = cv.StateID,
                                  })
            .ToListAsync();

        await _cache.Set(string.Format(RedisKeys.GetCurrencyDailyData, currency), currencyData);
        return result;
    }
    public async Task<ServiceResult<CurrencySelectListItemCachingResponse>> GetCurrencySelectListWithSlug()
    {
        var result = new ServiceResult<CurrencySelectListItemCachingResponse>();
        var cacheData = await _cache.Get<List<SelectListItem>>(RedisKeys.CurrencySelectListWithSlug);
        if (cacheData == null)
        {
            await RefreshCurrencySelectListWithSlug();
            cacheData = await _cache.Get<List<SelectListItem>>(RedisKeys.CurrencySelectListWithSlug);
        }

        result.Data = new CurrencySelectListItemCachingResponse
        {
            CurrenctSelectListItem = cacheData
        };

        return result;
    }
    public async Task<ServiceResult<CurrencySelectListItemCachingResponse>> GetCurrencySelectListWithID()
    {
        var result = new ServiceResult<CurrencySelectListItemCachingResponse>();
        var cacheData = await _cache.Get<List<SelectListItem>>(RedisKeys.CurrencySelectListWithID);
        if (cacheData == null)
        {
            await RefreshCurrencySelectListWithID();
            cacheData = await _cache.Get<List<SelectListItem>>(RedisKeys.CurrencySelectListWithID);
        }

        result.Data = new CurrencySelectListItemCachingResponse
        {
            CurrenctSelectListItem = cacheData
        };

        return result;
    }
    public async Task<ServiceResult<List<CurrencyCachingResponse>>> GetCurrencies()
    {
        var result = new ServiceResult<List<CurrencyCachingResponse>>();
        var cacheData = await _cache.Get<List<CurrencyCachingResponse>>(RedisKeys.GetCurrencyData);
        if (cacheData == null)
        {
            await RefreshCurrencies();
            cacheData = await _cache.Get<List<CurrencyCachingResponse>>(RedisKeys.GetCurrencyData);
        }

        result.Data = cacheData;

        return result;
    }
    public async Task<ServiceResult<List<CurrencyDailyDataCachingResponse>>> GetCurrencyDailyData(string currency)
    {
        var result = new ServiceResult<List<CurrencyDailyDataCachingResponse>>();
        var cacheData = await _cache.Get<List<CurrencyDailyDataCachingResponse>>(string.Format(RedisKeys.GetCurrencyDailyData, currency));
        if (cacheData == null)
        {
            await RefreshCurrencyDailyData(currency);
            cacheData = await _cache.Get<List<CurrencyDailyDataCachingResponse>>(string.Format(RedisKeys.GetCurrencyDailyData, currency));
        }

        result.Data = cacheData;

        return result;
    }
    public async Task<ServiceResult<GetTodayWeatherDataCachingResponse>> GetTodayWeatherData(int cityID)
    {
        var result = new ServiceResult<GetTodayWeatherDataCachingResponse>();
        var cacheData = await _cache.Get<GetTodayWeatherDataCachingResponse>(string.Format(RedisKeys.GetTodayWeatherDailyData, cityID));
        if (cacheData == null)
        {
            await RefreshTodayWeatherData();
            cacheData = await _cache.Get<GetTodayWeatherDataCachingResponse>(string.Format(RedisKeys.GetTodayWeatherDailyData, cityID));
        }

        result.Data = cacheData;

        return result;
    }
    public async Task<ServiceResult<List<WeatherData>>> GetAllWeatherData(int cityID)
    {
		var result = new ServiceResult<List<WeatherData>>();
		var cacheData = await _elasticSearchService.GetDocumentAsync<GetAllWeatherDataCachingResponse>(cityID.ToString(),IndexNames.WeatherData);
        if (cacheData == null)
        {
            await RefreshAllWeatherData();
            cacheData = await _elasticSearchService.GetDocumentAsync<GetAllWeatherDataCachingResponse>(cityID.ToString(), IndexNames.WeatherData);
        }
        
        result.Data = cacheData?.WeatherDatas;
        return result;
    }
    public async Task<ServiceResult> RefreshTodayWeatherData()
    {
        var result = new ServiceResult();

        string formattedDate = DateTime.Now.ToString("dd.MM.yyyy");

        var cities = await _context.Cities.AsNoTracking().ToListAsync();

        foreach (var city in cities)
        {
            var weatherData = await _context.WeatherData
                .Where(x => x.CityID == city.ID 
                            && x.Date == formattedDate)
                .Select(x => new GetTodayWeatherDataCachingResponse
                {
                    CityName = city.Name,
                    Degree = x.Degree.ToSplitDot(),
                    Status = x.Description.FirstCharToUpper(),
                }).FirstOrDefaultAsync();

            if (weatherData != null)
            {
                await _cache.Set(string.Format(RedisKeys.GetTodayWeatherDailyData, city.ID), weatherData);
            }
        }
        
        return result;
    }
    public async Task<ServiceResult> RefreshAllWeatherData()
    {
        var result = new ServiceResult();
        var cities = await _context.Cities.AsNoTracking().ToListAsync();
        var weatherDataList = new List<GetAllWeatherDataCachingResponse>();
        foreach (var city in cities)
        {
            var weatherData = await _context.WeatherData
                .Where(x => x.CityID == city.ID)
                .ToListAsync();

            var cachingData = new GetAllWeatherDataCachingResponse()
            {
                ID = city.ID,
                WeatherDatas = weatherData
            };
            weatherDataList.Add(cachingData);

		}
        if (weatherDataList.Any())
        {
			var deleteWeatherIndex = await _elasticSearchService.DeleteIndexAsync(IndexNames.WeatherData);
            if (deleteWeatherIndex)
            {
                var bulkdataWeatherDocument = await _elasticSearchService.BulkIndexDocumentsAsync(weatherDataList, IndexNames.WeatherData);
            }
		}

		await RefreshTodayWeatherData();
        return result;
    }
    public async Task<ServiceResult> RefreshLatestCurrencyNews()
    {
        var result = new ServiceResult();
        
        var news = await (
            from np in _context.NewsPublished
            join n in _context.News on np.NewsID equals n.ID
            let photo = (from ph in _context.NewsPublishedPhotos
                where ph.NewsID == np.NewsID &&
                      (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                       ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                orderby
                    ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                        ? 0
                        : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                            ? 1
                            : 2
                select ph).FirstOrDefault()
            where np.CategoryID == (int)CategoryEnum.Currency
            orderby n.CreatedDate descending
            select new GetLatestCurrencyNews
            {
                ID = (int)np.NewsID,
                Headline = np.Headline ?? np.Title,
                UpSubtitle = np.UpSubtitle ?? np.Subtitle ?? np.Content.ExtractTextFromHtml(),
                Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                    _appSettingsKeys.CDN_MediaGalleryFolder),
            }).Take(10).ToListAsync();
        
        await _cache.Set(RedisKeys.LatestCurrencyNews, news);
        return result;
    }
    public async Task<ServiceResult<List<GetLatestCurrencyNews>>> GetLatestCurrencyNews()
    {
        var result = new ServiceResult<List<GetLatestCurrencyNews>>();
        var cacheData = await _cache.Get<List<GetLatestCurrencyNews>>(RedisKeys.LatestCurrencyNews);
        if (cacheData == null)
        {
            await RefreshLatestCurrencyNews();
            cacheData = await _cache.Get<List<GetLatestCurrencyNews>>(RedisKeys.LatestCurrencyNews);
        }

        result.Data = cacheData;

        return result;
    }


}