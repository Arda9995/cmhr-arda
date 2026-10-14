using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Quark.API.Business.Interfaces;
using System.Text.Json;
using Quark.API.Business.Models.Requests.CommonRequest;

namespace Quark.Web.Controllers;

public class CommonController(ICommonService commonService) : CumhuriyetBaseController
{
    public IActionResult Index()
    {
        return View();
    }

    [OutputCache(PolicyName = "DynamicTTL")]
    public async Task<IActionResult> Currency(int count, string currency)
    {
        var result = await commonService.GetCurrencyPage(count, currency);
        return View(result.Data);
    }

    [OutputCache(PolicyName = "DynamicTTL")]
    public async Task<IActionResult> Weather()
    {
        var result = await commonService.GetWeatherData();
        return View(result.Data);
    }

    [OutputCache(PolicyName = "DynamicTTL")]
    public async Task<IActionResult> WeatherDetail(string city)
    {
        var result = await commonService.GetWeatherDetail(city);
        return View(result.Data);
    }

    [OutputCache(PolicyName = "DynamicTTL")]
    public async Task<IActionResult> PrayerTimes(string city)
    {
        var result = await commonService.GetPrayerTimes(city);
        return View(result.Data);
    }


    [HttpGet]
    [OutputCache(PolicyName = "DynamicTTL")]
    public async Task<IActionResult> ReadList()
    {
        var result = await commonService.GetReadList();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> GetReadListItem([FromBody] JsonElement data)
    {
        string readList = string.Empty;
        int page = 1;
        int pageSize = 10;

        if (data.TryGetProperty("readList", out JsonElement readListElement))
        {
            readList = readListElement.GetString();
        }

        if (data.TryGetProperty("page", out JsonElement pageElement))
        {
            page = pageElement.GetInt32();
        }

        var result = await commonService.GetReadListItem(readList, page, pageSize);

        return Json(new
        {
            items = result.Data.Items,
            hasMoreItems = result.Data.HasMoreItems
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> AddSetrowanSubscription(CreateWebSetrowSubscriptionRequest subscriptionRequest)
    {
        var result = await commonService.CreateSetrowSubscription(subscriptionRequest);

        return Json(new
        {
            isSuccess = result.IsSuccess,
            message = result.Message
        });
    }
}