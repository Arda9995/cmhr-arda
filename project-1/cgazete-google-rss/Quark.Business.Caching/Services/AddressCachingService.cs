using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class AddressCachingService : BaseService, IAddressCachingService
{
    public AddressCachingService(IHttpContextAccessor httpContext, QuarkDataContext context,
        IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(
        httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult<List<SelectListItem>>> Countries()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get<List<SelectListItem>>(RedisKeys.Countries);
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<SelectListItem>>(RedisKeys.Countries);
        }

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> CitiesByCountryId(int countryId)
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get<List<SelectListItem>>(string.Format(RedisKeys.Cities, countryId));
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<SelectListItem>>(string.Format(RedisKeys.Cities, countryId));
        }

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> CitiesOfTurkey()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get<List<SelectListItem>>(string.Format(RedisKeys.Cities, 90));
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<SelectListItem>>(string.Format(RedisKeys.Cities, 90));
        }

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> DistrictsByCityId(int cityId)
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get<List<SelectListItem>>(string.Format(RedisKeys.Districts, cityId));
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<SelectListItem>>(string.Format(RedisKeys.Districts, cityId));
        }

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> NeighbourhoodsByDistrictId(int districtId)
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get<List<SelectListItem>>(string.Format(RedisKeys.Neighbourhoods, districtId));
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<SelectListItem>>(string.Format(RedisKeys.Neighbourhoods, districtId));
        }

        return result;
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        #region All Data
        var countries = await _context.Countries.AsNoTracking().ToListAsync();
        var cities = await _context.Cities.AsNoTracking().ToListAsync();
        var districts = await _context.Districts.AsNoTracking().ToListAsync();
        var neighbourhoods = await _context.Neighbourhoods.AsNoTracking().ToListAsync();
        #endregion

        #region Countries
        var allCountries = (from s in countries
            orderby s.Order
            select new SelectListItem()
            {
                Value = s.ID.ToString(),
                Text = s.Name
            }).ToList();
        await _cache.Set_SelectListItems(RedisKeys.Countries, allCountries);
        #endregion

        #region Cities
        foreach (var country in countries)
        {
            var citiesOfCountry = (from s in cities
                where s.CountryID == country.ID
                select new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name
                }).ToList();
            if (citiesOfCountry.Count > 0)
            {
                await _cache.Set_SelectListItems(string.Format(RedisKeys.Cities, country.ID), citiesOfCountry);
            }
        }
        #endregion

        #region Districts
        foreach (var city in cities)
        {
            var districtsOfCity = (from s in districts
                where s.CityID == city.ID
                select new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name
                }).ToList();
            if (districtsOfCity.Count > 0)
            {
                await _cache.Set_SelectListItems(string.Format(RedisKeys.Districts, city.ID), districtsOfCity);
            }
        }
        #endregion

        #region Neighbourhoods
        foreach (var district in districts)
        {
            var neighbourhoodsOfDistrict = (from s in neighbourhoods
                where s.DistrictID == district.ID
                select new SelectListItem()
                {
                    Value = s.ID.ToString(),
                    Text = s.Name
                }).ToList();
            if (neighbourhoodsOfDistrict.Count > 0)
            {
                await _cache.Set_SelectListItems(string.Format(RedisKeys.Neighbourhoods, district.ID), neighbourhoodsOfDistrict);
            }
        }
        #endregion

        return result;
    }

 
}