using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CategoryRequests;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.CATEGORIES)]
public class CategoriesController : BaseController
{
    private readonly ICategoryService _service;
    
    public CategoriesController(ICategoryService service)
    {
        _service = service;
    }
    
    public IActionResult List()
    {
        return View();
    }
    
    public IActionResult Create(int? id)
    {
        var result = _service.Create(id);
        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateUpdateRequest request)
    {
        var result = await _service.Create(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return RedirectToAction(nameof(List));
    }
    
    public async Task<IActionResult> Update(int id)
    {
        var result = await _service.Update(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction(nameof(List));
        }
        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(CreateUpdateRequest request)
    {
        var result = await _service.Update(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return RedirectToAction(nameof(List));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return Json(result);
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var result = await _service.Details(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction(nameof(List));
        }

        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetNewsPaged(GetCategoryFiltersRequest request)
    {
        var result = await _service.GetNewsPaged(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }
    
    [HttpGet]
    public async Task<JsonResult> LoadTree(int? parent, bool onlyEnableds)
    {
        var result = await _service.GetCategoriesTree(parent, onlyEnableds);
        return Json(result.Data);
    }
    
    [HttpGet]
    public async Task<IActionResult> ChangeStatus(int id, bool isEnabled)
    {
        var result = await _service.ChangeCategoryStatus(id, isEnabled);
        return Json(result);
    }
}