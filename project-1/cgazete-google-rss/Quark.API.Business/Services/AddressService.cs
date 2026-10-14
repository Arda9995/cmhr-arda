using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.API.Business.Services;

public class AddressService : BaseService, IAddressService
{
    private readonly IAddressCachingService _addressCachingService;

    public AddressService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IAddressCachingService addressCachingService) : base(httpContext,
        context, localizer, appSettingsKeys, cache)
    {
        _addressCachingService = addressCachingService;
    }

    public async Task<ServiceResult<List<SelectListItem>>> Countries()
    {
        return await _addressCachingService.Countries();
    }

    public async Task<ServiceResult<List<SelectListItem>>> CitiesByCountryId(int countryId)
    {
        return await _addressCachingService.CitiesByCountryId(countryId);
    }

    public async Task<ServiceResult<List<SelectListItem>>> CitiesOfTurkey()
    {
        return await _addressCachingService.CitiesOfTurkey();
    }

    public async Task<ServiceResult<List<SelectListItem>>> DistrictsByCityId(int cityId)
    {
        return await _addressCachingService.DistrictsByCityId(cityId);
    }

    public async Task<ServiceResult<List<SelectListItem>>> NeighbourhoodsByDistrictId(int districtId)
    {
        return await _addressCachingService.NeighbourhoodsByDistrictId(districtId);
    }
}