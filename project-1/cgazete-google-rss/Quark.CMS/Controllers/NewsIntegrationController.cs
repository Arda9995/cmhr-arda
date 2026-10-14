using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.NewsIntegrationRequest;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.NEWS_INTEGRATION)]
public class NewsIntegrationController(INewsIntegrationService newsIntegrationService) : BaseController
{
    [HttpGet]
    public IActionResult List()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPaged()
    {
        var result = await newsIntegrationService.GetAllPagedAsync(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> Create(long? newsId)
    {
        var result = await newsIntegrationService.CreateAsync(newsId);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction("List");
        }
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateNewsIntegrationRequest request)
    {
        var result = await newsIntegrationService.CreateAsync(request);
        Message(result);
        return RedirectToAction("List");
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(long newsIntegrationId)
    {
        var result = await newsIntegrationService.UpdateAsync(newsIntegrationId);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction("List");
        }
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateNewsIntegrationRequest request)
    {
        var result = await newsIntegrationService.UpdateAsync(request);
        Message(result);
        return RedirectToAction("List");
    }
}