using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.MenuRequests;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.MENU)]
public class MenusController : BaseController
{
    private IMenuService _service;
    public MenusController(IMenuService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult List()
    {
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _service.Details(id);
        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create(int menuId, int? currentId)
    {
        var result = _service.Create(menuId, currentId);
        return View(result.Data);
    }
    
    [HttpGet]
    public IActionResult CreateParentMenu()
    {
        var result = _service.CreateParentMenu();
        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateParentMenu(CreateUpdateRequest request)
    {
        var result = await _service.CreateParentMenu(request);
        if (!result.IsSuccess)
        {
            Message(result);
            return View(result.Data);
        }
        
         return Redirect("/Menus/List");
        
    }
    

    [HttpPost]
    public async Task<IActionResult> Create(CreateUpdateRequest request)
    {
        var result = await _service.Create(request);
        if (!result.IsSuccess)
        {
            Message(result);
            return View(result.Data);
        }

        Message(result);
        
        return RedirectToAction(nameof(Details),new {id = request.DetailsID});
        
    }

    [HttpGet]
    public async Task<JsonResult> LoadTree(int? parent, bool onlyEnableds)
    {
        var result = await _service.GetMenusTree(parent, onlyEnableds);
        return Json(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> ChangeStatus(int id, bool isEnabled)
    {
        var result = await _service.ChangeMenuStatus(id, isEnabled);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        Message(result);
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> UpdateParent(int Id)
    {
        var model = await _service.UpdateParent(Id);
        return View(model.Data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateParent(CreateUpdateRequest request)
    {
        var result = await _service.UpdateParent(request);
        if (!result.IsSuccess)
        {
            Message(result);
            return View(result.Data);
        }
        Message(result);
        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPagedAsync()
    {
        var result = await _service.GetAllPaged(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(int menuId, int currentId)
    {
        var result = await _service.Update(menuId, currentId);
        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(CreateUpdateRequest request)
    {
        var result = await _service.Update(request);
        if (!result.IsSuccess)
        {
            Message(result);
            return View(result.Data);
        }

        Message(result);
        return RedirectToAction(nameof(Details),new {id = request.DetailsID});
    }
}