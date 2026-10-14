using AvvaMobile.Core;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CropperRequests;

namespace Quark.CMS.Controllers;

public class CropperController : BaseController
{
    private readonly ICropperService _cropperService;
    public CropperController(ICropperService cropperService)
    {
        _cropperService = cropperService;
    }
    public IActionResult Edit([FromQuery] EditRequest request)
    {
        return View(request);  
    }
    public IActionResult PhotoEdit([FromQuery] EditRequest request)
    {
        return View(request);
    }

    public IActionResult EditImage([FromBody] Base64ImageRequest request)
    {
        var response = _cropperService.EditImage(request);
        return Json(response.Data.PhotoURL);
    }
}