using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Models.Requests.TagRequests;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CommonRequests;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.TAGS)]
public class TagsController : BaseController
{
    private readonly ITagService _service;

    public TagsController(ITagService service)
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

    public async Task<IActionResult> DownloadExcelExport([FromQuery] ExcelExportRequest request)
    {
        var response = await _service.DownloadExcelExportAsync(request);

        if (response.IsSuccess)
        {
            return File(response.Data.FileBytes, response.Data.ContentType, response.Data.FileName);
        }

        return Json(response);
    }
    public IActionResult Create()
    {
        var result = _service.Create();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTagRequest request)
    {
        var result = await _service.Create(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/Tags/List");
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTagFromNewsModal(CreateTagRequest request)
    {
        var result = await _service.Create(request);
        return Json(result);
    }

    public async Task<IActionResult> Update(long id)
    {
        var result = await _service.Update(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return Redirect("/Tags/Update");
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateTagRequest request)
    {
        var result = await _service.Update(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/Tags/List");
    }

    public async Task<IActionResult> Delete(long id)
    {
        var result = await _service.Delete(id);
        return Json(result);
    }

    public async Task<IActionResult> Details(long id)
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
    public async Task<IActionResult> GetNewsPaged(GetTagFiltersRequest request)
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
    public async Task<IActionResult> Merge(MergeTagRequest request)
    {
        var result = await _service.Merge(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(request);
        }

        return Redirect("/Tags/List");
    }
    
    public async Task<IActionResult> SearchTags(string searchTerm)
    {
        var result = await _service.TagSearch(searchTerm);

        return Json(result.Data);
    }   
    
    public async Task<IActionResult> TagSearchByID(int id)
    {
        var result = await _service.TagSearchByID(id);

        return Json(result.Data);
    }
}