using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;

namespace Quark.API.Controllers;

[ApiController]
public class MenuController(IMenuService menuService) : ApiPublicBaseController
{
    [HttpGet]
    public async Task<IActionResult> ListByKey([FromQuery] string key)
    {
        var result = await menuService.ListMenusByKeyAsync(key);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }       

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await menuService.ListMenusAsync();
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        return Ok(result);
    }
}