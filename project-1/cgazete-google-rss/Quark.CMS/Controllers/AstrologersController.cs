using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.AstrologerRequests;

namespace Quark.CMS.Controllers;

public class AstrologersController : BaseController
{
    private readonly IAstrologerService _service;

    public AstrologersController(IAstrologerService service)
    {
        _service = service;
    }
    
    public IActionResult List()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPagedAsync()
    {
        var result = await _service.GetAllPaged(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);
    }
    
    public IActionResult Create()
    {
        var result = _service.Create();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAstrologerRequest request)
    {
        var result = await _service.Create(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/Astrologers/List");
    }

    public async Task<IActionResult> Update(int id)
    {
        var result = await _service.Update(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return Redirect("/Astrologers/Update");
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateAstrologerRequest request)
    {
        var result = await _service.Update(request);
        Message(result);
        if (result.IsSuccess)
        {
            return Redirect("/Astrologers/List");
        }

        return Redirect("/Astrologers/List");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
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
    public async Task<IActionResult> GetNewsPaged(GetAstrologerFiltersRequest request)
    {
        var result = await _service.GetNewsPaged(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }
}