using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using System.Text.Json;
using AvvaMobile.Core.APIControllers;

namespace Quark.API.Controllers;

[ApiController]
public class CommonController(ICommonService commonService) : ApiPublicBaseController
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Currency([FromQuery] int count, [FromQuery] string currency)
    {
        var result = await commonService.GetCurrencyPage(count, currency);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Weather()
    {
        var result = await commonService.GetWeatherData();
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> WeatherDetail(string city)
    {
        var result = await commonService.GetWeatherDetail(city);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTodayWeatherData()
    {
        var result = await commonService.GetTodayWeatherData();
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTodayCurrencyData()
    {
        var result = await commonService.GetTodayCurrencyData();
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> PrayerTimes(string city)
    {
        var result = await commonService.GetPrayerTimes(city);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> ReadList()
    {
        var result = await commonService.GetReadList();
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrencyList()
    {
        var result = await commonService.GetCurrencies();
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }
 
    
}