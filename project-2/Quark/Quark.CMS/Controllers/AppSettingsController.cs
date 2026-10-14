using AvvaMobile.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.Data.Models;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.APPSETTINGS)]
public class AppSettingsController : BaseController
{
    private readonly IAppSettingsService _appSettingsService;

    public AppSettingsController(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService;
    }
        
    public async Task<IActionResult> Update()
    {
        var result = await _appSettingsService.Update();
        return View(result.Data);
    }
        
    [HttpPost]
    public async Task<IActionResult> Update(List<AppSettingsGroup> request)
    {
        var result = await _appSettingsService.Update(request);
        Message(result);
        return Redirect("/AppSettings/Update");
    }
}