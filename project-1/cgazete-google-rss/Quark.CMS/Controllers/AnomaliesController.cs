using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.AnomalyRequests;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.ANOMALY)]
public class AnomaliesController : BaseController
{
    private readonly IAnomalyService _service;

    public AnomaliesController(IAnomalyService service)
    {
        _service = service;
    }

    public async Task<IActionResult> List()
    {
        var result = await _service.GetAllPagedFilters();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPaged(ListFiltersRequest request)
    {
        var result = await _service.GetAllPaged(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var response = await _service.Details(id);

        return View(response.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> MarkAsResolved(int id)
    {
        var response = await _service.MarkAsResolved(id);
        
        return Json(response);
    }
}