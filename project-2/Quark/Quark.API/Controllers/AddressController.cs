using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;

namespace Quark.API.Controllers;

public class AddressController(IAddressService addressService) : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> Countries()
    {
        var result = await addressService.Countries();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> CitiesOfTurkey()
    {
        var result = await addressService.CitiesOfTurkey();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> CitiesByCountryId(int id)
    {
        var result = await addressService.CitiesByCountryId(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> DistrictsByCityId(int id)
    {
        var result = await addressService.DistrictsByCityId(id);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> NeighbourhoodsByDistrictId(int id)
    {
        var result = await addressService.NeighbourhoodsByDistrictId(id);
        return Ok(result);
    }
}