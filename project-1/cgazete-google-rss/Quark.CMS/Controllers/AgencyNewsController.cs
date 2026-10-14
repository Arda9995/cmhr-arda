using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.AgencyNewsRequests;
using Quark.Resources;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.NEWS)]
public class AgencyNewsController(IAgencyNewsService _service, IStringLocalizer<CommonResource> localizer, ICategoryService _categoryService, IWebHostEnvironment _env)
    : BaseController
{
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
    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var result = await _service.Detail(id);
        return Json(result);
    }
}