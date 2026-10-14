using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nest;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.CommonResponses;
using Quark.API.Business.Models.Responses.LocationResponses;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;

namespace Quark.API.Business.Services
{
    public class LocationService : BaseService, ILocationService
    {
        private readonly ICommonCachingService _commonCachingService;
        public LocationService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, ICommonCachingService commonCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _commonCachingService = commonCachingService;
        }

        public async Task<ServiceResult<List<LocationResponse>>> GetCitiesByCountryAsync(int countryId)
        {
            var result = new ServiceResult<List<LocationResponse>>();

            var cities = await _commonCachingService.GetCities();

            result.Data = cities.Data
                .Where(x=>x.CountryID == countryId)
                .OrderBy(x => x.Name)
                .Select(x => new LocationResponse
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();

            return result;
        }

        public async Task<ServiceResult<List<LocationResponse>>> GetDistrictsByCityAsync(int cityId)
        {
            var result = new ServiceResult<List<LocationResponse>>();

            var districts = await _commonCachingService.GetDistricts();

            result.Data = districts.Data
                      .Where(x => x.CityID == cityId)
                      .OrderBy(x => x.Name)
                      .Select(x => new LocationResponse
                      {
                          Id = x.Id,
                          Name = x.Name
                      })
                      .ToList();

            return result;
        }
    }
} 