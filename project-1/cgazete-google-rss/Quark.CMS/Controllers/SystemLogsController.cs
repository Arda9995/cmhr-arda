using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.SystemLogRequests;

namespace Quark.CMS.Controllers
{
    [Authorize(Roles = SecurityClaimKeys.USERS)]
    public class SystemLogsController(ISystemLogService _service) : BaseController
    {
        public async Task<IActionResult> List()
        {
            var result = await _service.GetAllPagedFiltersAsync();
            return View(result.Data);
        }
        [HttpPost]
        public async Task<IActionResult> GetAllPagedAsync(ListFiltersRequest request)
        {
            var result = await _service.GetAllPagedAsync(new BaseDataTableRequest(Request), request);
            Message(result);
            return Json(result);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var response = await _service.Details(id);

            return View(response.Data);
        }
    }
}