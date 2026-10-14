using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Data.Models.Common;
using Quark.Hangfire.Business.Interfaces;
using Quark.Resources;
using System.Net.Http.Headers;
using System.Text.Json;
using Quark.Business.Caching.Interfaces;

namespace Quark.Hangfire.Business.Services;

public class PrayerTimesHangfireService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    ICommonCachingService commonCachingService,
    IConfiguration configuration)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), IPrayerTimesHangfireService
{
    private readonly HttpClient _httpClient = new();
    private readonly IConfiguration _configuration = configuration;
    private readonly string _baseUrl = "https://vakit.vercel.app/api/timesFromPlace";

    public async Task RefreshPrayerTimes()
    {
        var cities = await _context.Cities.ToListAsync();

        foreach (var city in cities)
        {
            try
            {
                var prayerTimes = await GetPrayerTimes(city.Name);
                if (prayerTimes != null && prayerTimes.times != null)
                {
                    await SavePrayerTimesData(prayerTimes, city.ID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing prayer times for {city.Name}: {ex.Message}");
            }
        }

        await commonCachingService.RefreshPrayerTimes();
    }

    private async Task<PrayerTimesApiResponse> GetPrayerTimes(string city)
    {
        try
        {
            string country = "Turkey";
            string region = city;
            int days = 30;
            int timezoneOffset = 180;
            string calculationMethod = "Turkey";

            var requestUrl =
                $"{_baseUrl}?country={country}&region={region}&city={city}&days={days}&timezoneOffset={timezoneOffset}&calculationMethod={calculationMethod}";

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PrayerTimesApiResponse>(content);
                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching prayer times: {ex.Message}");
            return null;
        }
    }

    private async Task SavePrayerTimesData(PrayerTimesApiResponse prayerTimesData, int cityID)
    {
        try
        {
            foreach (var timePair in prayerTimesData.times)
            {
                string date = timePair.Key;
                string[] times = timePair.Value;

                if (times.Length >= 6)
                {
                    var existingData = await _context.PrayerTimes.FirstOrDefaultAsync(p => p.CityID == cityID && p.Date == date);

                    if (existingData != null)
                    {
                        // Update existing record
                        existingData.Fajr = times[0]; // Fajr (Imsak)
                        existingData.Sunrise = times[1]; // Sunrise (Güneş)
                        existingData.Dhuhr = times[2]; // Dhuhr (Öğle)
                        existingData.Asr = times[3]; // Asr (İkindi)
                        existingData.Maghrib = times[4]; // Maghrib (Akşam)
                        existingData.Isha = times[5]; // Isha (Yatsı)

                        _context.PrayerTimes.Update(existingData);
                    }
                    else
                    {
                        // Create new record
                        var newPrayerTimes = new PrayerTimes
                        {
                            CityID = cityID,
                            Date = date,
                            Fajr = times[0], // Fajr (Imsak)
                            Sunrise = times[1], // Sunrise (Güneş)
                            Dhuhr = times[2], // Dhuhr (Öğle)
                            Asr = times[3], // Asr (İkindi)
                            Maghrib = times[4], // Maghrib (Akşam)
                            Isha = times[5] // Isha (Yatsı)
                        };

                        _context.PrayerTimes.Add(newPrayerTimes);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving prayer times data: {ex.Message}");
        }
    }
}

// Models for API response
public class PrayerTimesApiResponse
{
    public PlaceInfo place { get; set; }
    public Dictionary<string, string[]> times { get; set; }
}

public class PlaceInfo
{
    public string country { get; set; }
    public string countryCode { get; set; }
    public string city { get; set; }
    public string region { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}