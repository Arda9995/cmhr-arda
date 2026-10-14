using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.Business.Caching.Interfaces;

public interface IAddressCachingService
{
    Task<ServiceResult<List<SelectListItem>>> Countries();
    Task<ServiceResult<List<SelectListItem>>> CitiesByCountryId(int countryId);
    Task<ServiceResult<List<SelectListItem>>> CitiesOfTurkey();
    Task<ServiceResult<List<SelectListItem>>> DistrictsByCityId(int cityId);
    Task<ServiceResult<List<SelectListItem>>> NeighbourhoodsByDistrictId(int districtId);
   
    Task<ServiceResult> RefreshAll();
}