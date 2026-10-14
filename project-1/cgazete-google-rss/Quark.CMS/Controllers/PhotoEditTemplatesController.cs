using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.PhotoEditor;

namespace Quark.CMS.Controllers;

public class PhotoEditTemplatesController : BaseController
{
    IPhotoEditorTemplateService _photoEditorTemplateService;
    public PhotoEditTemplatesController(IPhotoEditorTemplateService photoEditorTemplateService) 
    {
        _photoEditorTemplateService = photoEditorTemplateService;
    }
    
    public async Task<IActionResult> List()
    {
        return View();
    }
    
    public async Task<IActionResult> GetAllPaged()
    {
        var result = await _photoEditorTemplateService.GetAllPaged(new BaseDataTableRequest(Request));
        return Json(result);
        ;
    }
    
    public async Task<IActionResult> Create()
    {
        var request = new CreateTemplateRequest();
        return View(request);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateTemplateRequest request)
    {
        var result =  _photoEditorTemplateService.CreateTemplateAsync(request).Result;
        return View("List");
    }

    [HttpPost]
    public async Task<IActionResult> PreparePhoto([FromForm]PrepareEditRequest request)
    {
        var result = await _photoEditorTemplateService.PrepareEditAsync(request);
        return Json(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var resul = await _photoEditorTemplateService.DeleteTemplateAsync(id);
        return Json(resul);
    }
    
    public async Task<IActionResult> Update(int id)
    {
        var request = _photoEditorTemplateService.UpdateTemplateAsync(id).Result;
        return View(request.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(UpdateTemplateRequest request)
    {
        var result =  _photoEditorTemplateService.UpdateTemplateAsync(request).Result;
        return View("List");
    }
    
}