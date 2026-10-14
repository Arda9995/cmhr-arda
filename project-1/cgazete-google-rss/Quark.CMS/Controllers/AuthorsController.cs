using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.AuthorRequests;
using Quark.CMS.Business.Models.Requests.CommonRequests;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.AUTHORS)]
public class AuthorsController : BaseController
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }
    public IActionResult List()
    {
        var result = _authorService.GetAllPagedFiltersAsync();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPagedAsync(GetFiltersRequest request)
    {
        var result = await _authorService.GetAllPagedAsync(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }

    public async Task<IActionResult> Create()
    {
        var result = await _authorService.Create();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUpdateRequest request)
    {
        var result = await _authorService.CreateAsync(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return Redirect("/Authors/List");
    }

    public async Task<IActionResult> Update(int id)
    {
        var result = await _authorService.UpdateAsync(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return Redirect("/Authors/List");
        }
        return View(result.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(CreateUpdateRequest request)
    {
        var result = await _authorService.UpdateAsync(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return Redirect("/Authors/List");
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _authorService.DetailsAsync(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction(nameof(List));
        }

        return View(result.Data);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _authorService.DeleteAsync(id);
        return Json(result);
    }
    public async Task<IActionResult> DownloadExcelExport([FromQuery] ExcelExportRequest request)
    {
        var response = await _authorService.DownloadExcelExportAsync(request);

        if (response.IsSuccess)
        {
            return File(response.Data.FileBytes, response.Data.ContentType, response.Data.FileName);
        }

        return Json(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetAllNewsPaged(DetailsAuthorRequest request)
    {
        var result = await _authorService.GetNewsPaged(new BaseDataTableRequest(Request), request);

        return Json(result);
    }
}