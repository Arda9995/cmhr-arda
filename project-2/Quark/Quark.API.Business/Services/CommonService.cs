using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Loging.Interfaces;
using AvvaMobile.Core.Loging.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.CommonResponses;
using Quark.API.Business.Models.Responses.NewsResponses;
using Quark.Business.Common.BaseServices;
using Quark.Core.Extentions;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using System.Globalization;
using Quark.API.Business.Models.Requests.CommonRequest;
using Quark.Business.Caching.Interfaces;
using Quark.Subscription.Adapter;
using Quark.Subscription.Adapter.Models.Requests.Setrow;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.API.Business.Services;

public class CommonService(
    QuarkDataContext context,
    IHttpContextAccessor httpContext,
    ICommonCachingService commonCachingService,
    INewsCachingService newsCachingService,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    ILogService logService,
    SetrowAdapter setrowAdapter,
    INewsService newsService)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), ICommonService
{
    public ServiceResult<List<SelectListItem>> GetDefaultSelectItems()
    {
        var result = new ServiceResult<List<SelectListItem>> { Data = new List<SelectListItem>() };
        result.Data.AddSELECTOption();
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetGenders()
    {
        var result = new ServiceResult<List<SelectListItem>>();
        var CurrentLanguageID = GetCurrentLanguageID(_localizer);

        var genders = await (from ct in _context.Genders.AsNoTracking()
            where ct.LanguageID == CurrentLanguageID
            orderby ct.ID
            select new SelectListItem { Text = ct.Name, Value = ct.ID.ToString() }).ToListAsync();
        genders.AddSELECTOption();

        result.Data = genders;
        return result;
    }

    public async Task<ServiceResult<List<GetCurrencyResponse>>> GetCurrencies()
    {
        var result = new ServiceResult<List<GetCurrencyResponse>>();

        var currencies = await (from ct in _context.Currencies.AsNoTracking()
            orderby ct.ID
            select new GetCurrencyResponse
            {
                DisplayName = ct.DisplayName,
                Code = ct.Code,
                Slug = ct.Slug,
                ID = ct.ID,
                Name = ct.Name
            }).ToListAsync();

        result.Data = currencies;
        return result;
    }

    public async Task<ServiceResult> TestElasticAsync()
    {
        var result = new ServiceResult();
        var model = new LogModel
        {
            CurrentUserID = CurrentUserID,
            CurrentUserName = CurrentUserName,
            ClassName = this.GetType().ToString(),
            Message = "Info Logu Denemesi",
            MethodName = GetMethodName(),
        };
        await logService.AddError(model, "TestElasticAsync");

        return result;
    }

    public async Task<ServiceResult<GetCurrencyPageResponse>> GetCurrencyPage(int count, string currency)
    {
        var result = new ServiceResult<GetCurrencyPageResponse>();

        var currencyCacheData = await commonCachingService.GetCurrencyDailyData(currency);
        var currencyData = currencyCacheData.Data
            .OrderByDescending(x => x.Date).Take(2).ToList();

        var todayData = currencyData.FirstOrDefault();
        var yesterdayData = currencyData.LastOrDefault();

        decimal? changePercent = null;
        if (yesterdayData != null && todayData.BuyingValue > 0)
        {
            changePercent = ((todayData.BuyingValue - yesterdayData.BuyingValue) / yesterdayData.BuyingValue) * 100;
        }

        var currenciesCache = await commonCachingService.GetCurrencies();
        var currencies = currenciesCache.Data.Select(x => new GetCurrencyResponse
        {
            DisplayName = x.DisplayName,
            Code = x.Code,
            Slug = x.Slug,
            ID = x.ID,
            Name = x.Name
        }).ToList();

        var currencyName = string.Empty;

        if (todayData == null)
        {
            currencyName = currencies.FirstOrDefault(x => x.Slug == currency).DisplayName;
        }

        var currenciesSelectListItem = currencies.Select(x => new SelectListItem
        {
            Text = x.DisplayName,
            Value = x.Slug,
            Selected = x.Slug == currency
        }).ToList();

        var mostViewedNews = await newsCachingService.GetMostViewNews();
        var categoryNews = mostViewedNews.Data
            .Select(x => new CategoryNews()
            {
                ID = x.ID,
                Headline = x.Headline,
                UpSubtitle = x.UpSubtitle,
                Slug = x.Slug,
                ImageURL = x.ImageURL
            })
            .Take(4).ToList();

        var latestCacheNews = await commonCachingService.GetLatestCurrencyNews();
        var latestCategoryCurrencyNews = latestCacheNews.Data.Take(4).Select(x => new CategoryNews()
        {
            ID = x.ID,
            Headline = x.Headline,
            UpSubtitle = x.UpSubtitle,
            Slug = x.Slug,
            ImageURL = x.ImageURL
        }).ToList();

        result.Data = new GetCurrencyPageResponse
        {
            Currencies = currencies,
            SelectedCurrencyID = todayData?.ID ?? 0,
            SelectedCurrencySlug = todayData?.Slug ?? string.Empty,
            BuyingValue = todayData?.BuyingValue * count ?? 0,
            SellingValue = todayData?.SellingValue * count ?? 0,
            SelectedCurrencyName = todayData.DisplayName ?? string.Empty,
            ChangePercent = changePercent,
            CategoryNews = categoryNews,
            CurrencyNews = latestCategoryCurrencyNews,
            Value = count,
            CurrencySelectListItems = currenciesSelectListItem
        };

        return result;
    }
    
    public async Task<ServiceResult<WeatherPageResponse>> GetWeatherData()
    {
        var result = new ServiceResult<WeatherPageResponse>();

        var citiesData = await commonCachingService.GetCities();
        var cities = citiesData.Data.Select(x => new Cities
        {
            ID = x.Id,
            Name = x.Name,
            Order = x.Order
        }).OrderBy(x => x.Order).ToList();


        var currentUrl = _httpContext.HttpContext.Request.Path.Value?.ToLower();

        var citySelectListItems = cities.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = "/hava-durumu/" + x.Name.ToSlugTR(),
            Selected = ("/hava-durumu/" + x.Name.ToSlugTR() == currentUrl)
        }).ToList();

        if (currentUrl == "/hava-durumu/" || string.IsNullOrEmpty(currentUrl))
        {
            citySelectListItems.Insert(0, new SelectListItem
            {
                Text = "Şehir Seçiniz",
                Value = "",
                Selected = true
            });
        }
        else
        {
            citySelectListItems.Insert(0, new SelectListItem
            {
                Text = "Şehir Seçiniz",
                Value = ""
            });
        }

        var cityIDs = cities
            .Where(x => x.Name == "İstanbul"
                        || x.Name == "Ankara"
                        || x.Name == "İzmir"
                        || x.Name == "Antalya")
            .Select(x => new { x.ID, x.Name })
            .ToList();

        var weatherDataDictionary = new Dictionary<string, List<WeatherDataResponse>>();

        foreach (var cityID in cityIDs)
        {
            var cacheData = await commonCachingService.GetAllWeatherData(cityID.ID);
            var weatherData = cacheData.Data
                .Where(w => w.CityID == cityID.ID)
                .OrderBy(w => w.Date)
                .Take(6)
                .Select(x => new WeatherDataResponse
                {
                    City = cityID.Name,
                    Max = x.Max.ToSplitDot(),
                    Date = x.Date,
                    Day = x.Day,
                    Degree = x.Degree,
                    Description = x.Description,
                    Humidity = x.Humidity,
                    IconURL = x.IconURL,
                    Min = x.Min.ToSplitDot(),
                    Night = x.Night,
                    Status = x.Status
                }).ToList();

            weatherDataDictionary[cityID.Name] = weatherData;
        }

        var mostViewedNews = await newsCachingService.GetMostViewNews();
        var categoryNews = mostViewedNews.Data
            .Select(x => new CategoryNews()
            {
                ID = x.ID,
                Headline = x.Headline,
                UpSubtitle = x.UpSubtitle,
                Slug = x.Slug,
                ImageURL = x.ImageURL
            })
            .Take(4).ToList();


        if (weatherDataDictionary == null)
        {
            result.SetError(_localizer["Weather data not found for the specified city."]);
            return result;
        }

        var response = new WeatherPageResponse
        {
            CategoryNews = categoryNews,
            Cities = cities,
            WeatherDataResponse = weatherDataDictionary,
            CitySelectListItems = citySelectListItems
        };


        result.Data = response;
        return result;
    }

    public async Task<ServiceResult<WeatherDetailPageResponse>> GetWeatherDetail(string city)
    {
        var result = new ServiceResult<WeatherDetailPageResponse>();

        var citiesData = await commonCachingService.GetCities();
        var cities = citiesData.Data.Select(x => new Cities
        {
            ID = x.Id,
            Name = x.Name,
            Order = x.Order
        }).OrderBy(x => x.Order).ToList();

        var citySelectListItems = cities.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = "/hava-durumu/" + x.Name.ToSlugTR(),
            Selected = ("/hava-durumu/" + x.Name.ToSlugTR() == "/hava-durumu/" + city)
        }).ToList();

        var cityIDs = cities
            .Where(x => x.Name.ToSlugTR() == city.ToSlugTR())
            .Select(x => new { x.ID, x.Name })
            .ToList();

        var weatherDataDictionary = new Dictionary<string, List<WeatherDataResponse>>();

        foreach (var cityID in cityIDs)
        {
            var cacheData = await commonCachingService.GetAllWeatherData(cityID.ID);
            var weatherData = cacheData.Data
                .Where(w => w.CityID == cityID.ID)
                .OrderBy(w => w.Date)
                .Take(6)
                .Select(x => new WeatherDataResponse
                {
                    City = cityID.Name,
                    Max = x.Max.ToSplitDot(),
                    Date = x.Date,
                    Day = x.Day,
                    Degree = x.Degree,
                    Description = x.Description,
                    Humidity = x.Humidity,
                    IconURL = x.IconURL,
                    Min = x.Min.ToSplitDot(),
                    Night = x.Night,
                    Status = x.Status
                }).ToList();

            weatherDataDictionary[cityID.Name] = weatherData;
        }

        var categoryNewsCacheData = await newsCachingService.GetMostViewNews();
        var categoryNews = categoryNewsCacheData.Data.Select(x => new CategoryNews()
            {
                ID = x.ID,
                Headline = x.Headline,
                UpSubtitle = x.UpSubtitle,
                Slug = x.Slug,
                ImageURL = x.ImageURL
            })
            .Take(4).ToList();


        if (weatherDataDictionary == null)
        {
            result.SetError(_localizer["Weather data not found for the specified city."]);
            return result;
        }


        var response = new WeatherDetailPageResponse
        {
            CategoryNews = categoryNews,
            Cities = cities,
            WeatherDataResponse = weatherDataDictionary,
            CitySelectListItems = citySelectListItems
        };


        result.Data = response;
        return result;
    }

    public async Task<ServiceResult<GetPrayerTimesResponse>> GetPrayerTimes(string city)
    {
        var result = new ServiceResult<GetPrayerTimesResponse>();

        var citiesData = await commonCachingService.GetCities();
        var cities = citiesData.Data.Select(x => new Cities
        {
            ID = x.CityID,
            Name = x.Name
        }).ToList();

        var currentCity = cities
            .FirstOrDefault(x => x.Name.ToSlugTR() == city);

        if (city == null)
        {
            currentCity = cities.FirstOrDefault(x => x.ID == 34);
        }

        var citySelectList = cities
            .Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = "/namaz-vakitleri/" + x.Name.ToSlugTR(),
                Selected = ("/namaz-vakitleri/" + x.Name.ToSlugTR() == "/namaz-vakitleri/" + city)
            })
            .ToList();

        var xId = currentCity.ID;
        var cachePrayerTimes = await commonCachingService.GetPrayerTimes(currentCity.ID);
        var prayerTimes = new List<PrayerTime>();
        if (cachePrayerTimes.IsSuccess && cachePrayerTimes.Data.Any())
        {
            prayerTimes = cachePrayerTimes.Data
                .Where(x => x.CityID == currentCity.ID).Select(x => new PrayerTime
                {
                    Asr = x.Asr,
                    Date = x.Date,
                    Fajr = x.Fajr,
                    Isha = x.Isha,
                    Maghrib = x.Maghrib,
                    Sunrise = x.Sunrise,
                    Dhuhr = x.Dhuhr,
                }).ToList();
        }
        else
        {
            prayerTimes = await _context.PrayerTimes.Where(x => x.CityID == currentCity.ID).Select(x => new PrayerTime
            {
                Asr = x.Asr,
                Date = x.Date,
                Fajr = x.Fajr,
                Isha = x.Isha,
                Maghrib = x.Maghrib,
                Sunrise = x.Sunrise,
                Dhuhr = x.Dhuhr,
            }).ToListAsync();
        }

        var mostViewedNews = await newsCachingService.GetMostViewNews();
        var categoryNews = mostViewedNews.Data
            .Select(x => new CategoryNews()
            {
                ID = x.ID,
                Headline = x.Headline,
                UpSubtitle = x.UpSubtitle,
                Slug = x.Slug,
                ImageURL = x.ImageURL
            })
            .Take(4).ToList();

        var response = new GetPrayerTimesResponse
        {
            CityName = currentCity.Name,
            Date = DateTime.Now.ToString("dd.MM.yyyy", CultureInfo.CurrentCulture),
            CategoryNews = categoryNews,
            PrayerTimes = prayerTimes,
            Slug = currentCity.Name.ToSlugTR(),
            Cities = citySelectList
        };
        result.Data = response;
        return result;
    }

    public async Task<ServiceResult<List<string>>> GetCityNames()
    {
        var result = new ServiceResult<List<string>>();
        var cities = await _context.Cities.Select(x => x.Name).ToListAsync();
        result.Data = cities;
        return result;
    }

    public async Task<ServiceResult<GetReadListPageResponse>> GetReadList()
    {
        var result = new ServiceResult<GetReadListPageResponse>();

        var mostSearchedNews = await newsService.GetMostSearchedNewsBlock();

        var mostViewedNews = await newsCachingService.GetMostViewNews();
        var mostViewedNewsModel = mostViewedNews.Data.Select(x => new CategoryNews
        {
            Headline = x.Headline,
            ID = x.ID,
            UpSubtitle = x.UpSubtitle,
            Slug = x.Slug,
            ImageURL = x.ImageURL,
        }).Take(4).ToList();


        var response = new GetReadListPageResponse
        {
            CategoryNews = mostViewedNewsModel,
            PageCachingResponses = mostSearchedNews.Data,
        };
        result.Data = response;
        return result;
    }

    public async Task<ServiceResult<ReadListItemsResponse>> GetReadListItem(string readList, int page, int pageSize)
    {
        var result = new ServiceResult<ReadListItemsResponse>
        {
            Data = new ReadListItemsResponse()
        };

        if (string.IsNullOrEmpty(readList))
        {
            return result;
        }

        var readListIDs = readList.Split(',').Select(x => long.Parse(x)).ToList();

        int skip = (page - 1) * pageSize;

        var itemsToFetch = pageSize + 1;

        var cachedList = new List<ReadListItem>();

        foreach (var item in readListIDs)
        {
            var newsDetail = await newsCachingService.GetNewsDetailByID((int)item);
            if (newsDetail.IsSuccess && newsDetail.Data != null)
            {
                cachedList.Add(new ReadListItem
                {
                    ID = (int)item,
                    Headline = newsDetail.Data.Headline ?? newsDetail.Data.Title,
                    UpSubtitle = newsDetail.Data.UpSubtitle.Truncate(80) ?? newsDetail.Data.SubTitle.Truncate(80) ??
                        newsDetail.Data.Content.ExtractTextFromHtml(80),
                    Slug = newsDetail.Data.NewsSlug,
                    ImageURL = newsDetail.Data.CoverPhoto,
                    PublishDate = newsDetail.Data.PublishDate.ToFormattedDateTime(),
                    SaveDate = DateTime.Now.ToString("dd.MM.yyyy"),
                });
            }
            else
            {
                var savedNews = await (
                    from np in _context.NewsPublished
                    let photo = (from ph in _context.NewsPhotos
                        where ph.NewsID == np.NewsID &&
                              (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                               ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                        orderby
                            ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                ?
                                0
                                :
                                ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                !string.IsNullOrEmpty(ph.PhotoUrl)
                                    ? 1
                                    : 2
                        select ph).FirstOrDefault()
                    where np.NewsID == item
                    select new ReadListItem
                    {
                        ID = (int)np.NewsID,
                        Headline = np.Headline ?? np.Title,
                        UpSubtitle = np.UpSubtitle.Truncate(80) ??
                                     np.Subtitle.Truncate(80) ?? np.Content.ExtractTextFromHtml(80),
                        Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                        ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                            _appSettingsKeys.CDN_MediaGalleryFolder),
                        SaveDate = DateTime.Now.ToString("dd.MM.yyyy"),
                        PublishDate = np.PublishedDate.ToString("dd.MM.yyyy"),
                    }).FirstOrDefaultAsync();
                if (savedNews != null)
                {
                    cachedList.Add(savedNews);
                }
            }
        }

        cachedList = cachedList.Skip(skip).Take(itemsToFetch).ToList();

        bool hasMoreItems = cachedList.Count > pageSize;

        if (hasMoreItems)
        {
            cachedList.RemoveAt(cachedList.Count - 1);
        }

        result.Data.Items = cachedList;
        result.Data.HasMoreItems = hasMoreItems;

        return result;
    }

    public async Task<ServiceResult<GetTodayWeatherDataResponse>> GetTodayWeatherData()
    {
        var result = new ServiceResult<GetTodayWeatherDataResponse>();

        var dailyWeatherData = await commonCachingService.GetTodayWeatherData(34);

        if (dailyWeatherData == null || dailyWeatherData.Data == null)
        {
            result.SetError(_localizer["Weather data not found."]);
            return result;
        }

        result.Data = new GetTodayWeatherDataResponse
        {
            CityName = dailyWeatherData.Data.CityName,
            Degree = dailyWeatherData.Data.Degree,
            Status = dailyWeatherData.Data.Status,
        };

        return result;
    }

    public async Task<ServiceResult<GetTodayCurrencyDataResponse>> GetTodayCurrencyData()
    {
        var result = new ServiceResult<GetTodayCurrencyDataResponse>();
        var allowedSlugs = new List<string> { "dolar", "euro", "sterlin", "bitcoin", "gram-altin" };
        var currencyData = new List<TodayCurrencyDataItem>();

        foreach (var allowedSlug in allowedSlugs)
        {
            var currencyCache = await commonCachingService.GetCurrencyDailyData(allowedSlug);

            var currencyDataItem = currencyCache.Data
                .OrderByDescending(x => x.Date).Select(x => new TodayCurrencyDataItem()
                {
                    CurrencyName = x.DisplayName,
                    CurrencyValue = x.SellingValue.ToString("N2"),
                    Status = x.StateID
                }).FirstOrDefault();

            currencyData.Add(currencyDataItem);
        }

        var response = new GetTodayCurrencyDataResponse
        {
            TodayCurrencyDataItems = currencyData
        };

        result.Data = response;
        return result;
    }

    public async Task<ServiceResult<string>> CreateSetrowSubscription(CreateWebSetrowSubscriptionRequest subscriptionRequest)
    {
        var result = new ServiceResult<string>();

        try
        {
            foreach (var groupId in subscriptionRequest.GroupIDs)
            {
                var request = new CreateSetrowSubscriptionRequest
                {
                    ApiKey = _appSettingsKeys.SETROW_API_KEY,
                    BaseUrl = _appSettingsKeys.SETROW_BASE_URL,
                    Email = subscriptionRequest.Email,
                    Name = subscriptionRequest.Name,
                    GroupID = groupId,
                };
            
                var responseString = await setrowAdapter.CreateSetrowSubscription(request);
                result.Data = responseString.Data;
            }
        }
        catch (Exception ex)
        {
            result.SetError(ex.Message);
        }

        return result;
    }
}