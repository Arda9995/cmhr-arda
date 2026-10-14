using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.SourceRequests;
using Quark.CMS.Business.Models.Requests.TagRequests;

namespace Quark.CMS.Controllers;

[Authorize]
public class SourcesController(ISourceService service) : BaseController
{
    private readonly ISourceService _service = service;
    
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
    public async Task<IActionResult> Create(CreateSourceRequest request)
    {
        var result = await _service.Create(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/Sources/List");
    }
    
    public async Task<IActionResult> Update(int id)
    {
        var result = await _service.Update(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return Redirect("/Sources/Update");
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateSourceRequest request)
    {
        var result = await _service.Update(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/Sources/List");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        return Json(result);
    }

    public async Task<IActionResult> SearchSources(string searchTerm)
    {
        var result = await _service.SearchSources(searchTerm);
        return Json(result);
    } 
    
    public async Task<IActionResult> SearchSourcesByID(int id)
    {
        var result = await _service.SearchSourcesByID(id);
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
    public async Task<IActionResult> GetNewsPaged(GetDetailsFiltersRequest request)
    {
        var result = await _service.GetNewsPaged(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }

    public async Task<IActionResult> Merge()
    {
        var result = await _service.Merge();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Merge(MergeSourceRequest request)
    {
        var result = await _service.Merge(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(request);
        }

        return Redirect("/Sources/List");
    }

    public async Task<IActionResult> SearchTags(string searchTerm)
    {
        var result = await _service.SourceSearch(searchTerm);

        return Json(result.Data);
    }
}