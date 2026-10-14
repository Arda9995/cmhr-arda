using AvvaMobile.Core.Business;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.CMS.Business.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;
using AvvaMobile.Core.Caching;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;
using Quark.CMS.Business.Models.Responses.CommonResponses;

namespace Quark.CMS.Business.Services;

public class CommonService : BaseService, ICommonService
{
    public CommonService(QuarkDataContext context, IHttpContextAccessor httpContext,
        IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) :
        base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

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

    public async Task<ServiceResult<List<SelectListItem>>> GetCountries()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var countries = await (from c in _context.Countries.AsNoTracking()
            orderby c.Order
            select new SelectListItem { Text = c.Name, Value = c.ID.ToString() }).ToListAsync();
        countries.AddSELECTOption();

        result.Data = countries;
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetCitiesOfTurkey()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var cachedData = await _cache.Get_SelectListItems("TurkeyCities");
        if (cachedData != null)
        {
            result.Data = cachedData;
        }
        else
        {
            result.Data = await (from c in _context.Cities.AsNoTracking()
                where c.CountryID == 90
                orderby c.Order
                select new SelectListItem { Text = c.Name, Value = c.ID.ToString() }).ToListAsync();

            await _cache.Set_SelectListItems("TurkeyCities", result.Data);
        }

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetCitiesByCountryId(int id)
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var cities = await (from c in _context.Cities.AsNoTracking()
            where c.CountryID == id
            orderby c.Order
            select new SelectListItem { Text = c.Name, Value = c.ID.ToString() }).ToListAsync();
        cities.AddSELECTOption();

        result.Data = cities;
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetDistrictsByCityID(int id)
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var districts = await (from c in _context.Districts.AsNoTracking()
            where c.CityID == id
            orderby c.Name
            select new SelectListItem { Text = c.Name, Value = c.ID.ToString() }).ToListAsync();
        districts.AddSELECTOption();

        result.Data = districts;
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetNeighbourByDistrictID(int id)
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var neighborhoods = await (from c in _context.Neighbourhoods.AsNoTracking()
            where c.DistrictID == id
            orderby c.Name
            select new SelectListItem { Text = c.Name, Value = c.ID.ToString() }).ToListAsync();
        neighborhoods.AddSELECTOption();

        result.Data = neighborhoods;
        return result;
    }

}
