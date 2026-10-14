using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.CommonResponses;
using Quark.Web.Models.ViewComponentResponses;

namespace Quark.Web.ViewComponents
{
    public class CommonViewComponent : ViewComponent
    {
        private readonly ICommonService _commonService;
        public CommonViewComponent(ICommonService commonService)
        {
            _commonService = commonService;
        }


        public async Task<IViewComponentResult> InvokeAsync(string methodName)
        {
            var currencyData = await _commonService.GetTodayCurrencyData();
            var weatherData = await _commonService.GetTodayWeatherData();

            var weatherAndCurrencyData = new GetWeatherAndCurrencyResponse
            {
                Currency = currencyData.Data,
                Weather = weatherData.Data
            };

            switch (methodName)
            {
                case "Currency":
                    return View($"~/Views/Shared/Components/Common/{methodName}.cshtml", currencyData.Data);
                case "MobileCurrency":
                    return View($"~/Views/Shared/Components/Common/{methodName}.cshtml", currencyData.Data);
                case "Weather":
                    return View($"~/Views/Shared/Components/Common/{methodName}.cshtml", weatherData.Data);
                case "CurrencyWeather":
                    return View($"~/Views/Shared/Components/Common/{methodName}.cshtml", weatherAndCurrencyData);
                default:
                    return View();
            }
        }
    }
}
