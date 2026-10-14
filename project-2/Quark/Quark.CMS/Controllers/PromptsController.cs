using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.PromptRequests;

namespace Quark.CMS.Controllers;

[Authorize]
public class PromptsController: BaseController
{
    private readonly IPromptService _promptService;

    public PromptsController(IPromptService promptService)
    {
        _promptService = promptService;
    }
    
    public async Task<IActionResult> List()
    {
        var result = await _promptService.GetAllPagedFilters();
        return View(result.Data);    
    }
    
    [HttpPost]
    public async Task<IActionResult> GetAllPaged(GetPromptFiltersRequest request)
    {
        var result = await _promptService.GetAllPaged(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }
    
    public async Task<IActionResult> Create()
    {
        var result = await _promptService.Create();
        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreatePromptRequest request)
    {
        var result = await _promptService.Create(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return RedirectToAction("List");
    }
    
    public async Task<IActionResult> Update(int id)
    {
        var result = await _promptService.Update(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction("List");
        }
        return View(result.Data);
    }
        
    [HttpPost]
    public async Task<IActionResult> Update(UpdatePromptRequest request)
    {
        var result = await _promptService.Update(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return RedirectToAction("List");
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _promptService.Delete(id);
        return Json(result);
    }
}