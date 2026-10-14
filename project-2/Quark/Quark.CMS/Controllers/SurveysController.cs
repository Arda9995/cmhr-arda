using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.SurveyRequests;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.USERS)]
public class SurveysController(ISurveyService _service) : BaseController
{
    public IActionResult List()
    {
        var result = _service.GetAllPagedFilters();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPagedAsync(GetSurveyFiltersRequest request)
    {
        var result = await _service.GetAllPaged(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }
    
    public IActionResult Create()
    {
        var result = _service.Create();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSurveyRequest request)
    {
        var result = await _service.Create(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/Surveys/Details");
    }

    public async Task<IActionResult> Update(int id)
    {
        var result = await _service.Update(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return Redirect("/Surveys/Update");
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateSurveyRequest request)
    {
        var result = await _service.Update(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/Surveys/List");
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
    public async Task<IActionResult> EnableSurvey(int id)
    {
        var result = await _service.EnableSurvey(id);
        
        return Json(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> RefreshAnswers(int id)
    {
        var response = await _service.RefreshAnswers(id);
        
        return Json(response);
    }
}