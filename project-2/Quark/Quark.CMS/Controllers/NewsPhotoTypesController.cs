using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CommonRequests;
using Quark.CMS.Business.Models.Requests.NewsPhotoTypeRequests;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.NEWS_PHOTO_TYPES)]
public class NewsPhotoTypesController : BaseController
{
    private readonly INewsPhotoTypeService _newsPhotoTypeService;

    public NewsPhotoTypesController(INewsPhotoTypeService newsPhotoTypeService)
    {
        _newsPhotoTypeService = newsPhotoTypeService;
    }

    public IActionResult List()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPagedAsync()
    {
        var result = await _newsPhotoTypeService.GetAllPagedAsync(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);
    }

    public async Task<IActionResult> DownloadExcelExport([FromQuery] ExcelExportRequest request)
    {
        var response = await _newsPhotoTypeService.DownloadExcelExportAsync(request);

        if (response.IsSuccess)
        {
            return File(response.Data.FileBytes, response.Data.ContentType, response.Data.FileName);
        }

        return Json(response);
    }
    public IActionResult Create()
    {
        var result = _newsPhotoTypeService.Create();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUpdateRequest request)
    {
        var result = await _newsPhotoTypeService.CreateAsync(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/NewsPhotoTypes/List");
    }

    public async Task<IActionResult> Update(int id)
    {
        var result = await _newsPhotoTypeService.UpdateAsync(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return Redirect("/NewsPhotoTypes/List");
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(CreateUpdateRequest request)
    {
        var result = await _newsPhotoTypeService.UpdateAsync(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/NewsPhotoTypes/List");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _newsPhotoTypeService.DeleteAsync(id);
        return Json(result);
    }
}