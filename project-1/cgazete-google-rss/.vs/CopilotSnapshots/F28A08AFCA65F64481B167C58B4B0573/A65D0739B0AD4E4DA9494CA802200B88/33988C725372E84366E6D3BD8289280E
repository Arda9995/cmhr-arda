using Quark.API.Business.Interfaces;
using Quark.Core.Extentions;

namespace Quark.Web.RouteConfigs
{
    public class CommonRouteConfig
    {
        public static async Task RegisterRoutes(IEndpointRouteBuilder app, WebApplication application)
        {
            var currencyList = new List<string>();
            var cityList = new List<string>();
            try
            {
                using var scope = application.Services.CreateScope();
                var commonService = scope.ServiceProvider.GetRequiredService<ICommonService>();
                var currencies = await commonService.GetCurrencies();
                var cities = await commonService.GetCityNames();
                if (currencies?.Data != null)
                    currencyList = currencies.Data.Select(c => c.Slug).ToList();
                if (cities?.Data != null)
                    cityList = cities.Data;
            }
            catch (Exception ex)
            {
                application.Services.GetService<ILogger<CommonRouteConfig>>()?
                    .LogError(ex, "Currency/city routes could not be loaded; continuing with remaining routes");
            }

            foreach (var currencySlug in currencyList)
            {
                app.MapControllerRoute(
                    name: $"Currency_{currencySlug}",
                    pattern: $"doviz/{{count:int}}-{currencySlug}-kac-tl",
                    defaults: new { controller = "Common", action = "Currency", currency = currencySlug }
                );
            }

                app.MapControllerRoute(
                 name: $"PrayerTimes_Default",
                 pattern: $"/namaz-vakitleri/",
                 defaults: new { controller = "Common", action = "PrayerTimes", city = "istanbul" }
             );


                app.MapControllerRoute(
                name: "Common_Weather",
                pattern: "hava-durumu/",
                defaults: new { controller = "Common", action = "Weather" }
            );

            foreach (var city in cityList)
            {
                var slug = city.ToSlugTR();
                app.MapControllerRoute(
                    name: $"Weather_{city}",
                    pattern: $"/hava-durumu/{slug}",
                    defaults: new { controller = "Common", action = "WeatherDetail", city = slug }
                );

                app.MapControllerRoute(
                    name: $"PrayerTimes_{city}",
                    pattern: $"/namaz-vakitleri/{slug}",
                    defaults: new { controller = "Common", action = "PrayerTimes", city = slug }
                );
            }

            app.MapControllerRoute(
                name: "Common_ReadList",
                pattern: "okuma-listem/",
                defaults: new { controller = "Common", action = "ReadList" }
            );

                    app.MapControllerRoute(
               name: "Dynamic_News_Search",
               pattern: "arama",
               defaults: new { controller = "News", action = "Search" }
           );

            app.MapControllerRoute(
                  name: "Dynamic_News_SearchByKey",
                  pattern: "arama/{search}",
                  defaults: new { controller = "News", action = "SearchedNews" }
              );
        }
    }
}
