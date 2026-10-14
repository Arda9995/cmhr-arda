using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;

namespace Quark.Web.Controllers
{
    public class LocationController : CumhuriyetBaseController
	{
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByCountry(int countryId)
        {
            var cities = await _locationService.GetCitiesByCountryAsync(countryId);
            return Json(cities.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetDistrictsByCity(int cityId)
        {
            var districts = await _locationService.GetDistrictsByCityAsync(cityId);
            return Json(districts.Data);
        }
    }
}