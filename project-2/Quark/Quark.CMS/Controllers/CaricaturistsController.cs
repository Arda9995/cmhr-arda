using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CaricaturistRequests;

namespace Quark.CMS.Controllers;

public class CaricaturistsController : BaseController
{
    private readonly ICaricaturistService _service;

    public CaricaturistsController(ICaricaturistService service)
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
    public async Task<IActionResult> Create(CreateCaricaturistRequest request)
    {
        var result = await _service.Create(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/Caricaturists/List");
    }

    public async Task<IActionResult> Update(int id)
    {
        var result = await _service.Update(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return Redirect("/Caricaturists/Update");
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateCaricaturistRequest request)
    {
        var result = await _service.Update(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(request);
        }

        return Redirect("/Caricaturists/List");
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
    public async Task<IActionResult> GetNewsPaged(GetCaricaturistFiltersRequest request)
    {
        var result = await _service.GetNewsPaged(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }
}