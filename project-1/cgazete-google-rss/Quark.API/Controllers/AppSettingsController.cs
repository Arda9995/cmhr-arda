using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;

namespace Quark.API.Controllers;

public class AppSettingsController(IAppSettingsService appSettingsService) : ApiPublicBaseController
{
    [HttpGet]
    public IActionResult AppVersions()
    {
        var result = appSettingsService.AppVersions();
        return Ok(result);
    }
}