using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.RoleRequests;

namespace Quark.CMS.Controllers
{
    [Authorize(Roles = SecurityClaimKeys.ROLES)]
    public class RolesController : BaseController
    {
        private readonly IRoleService _service;

        public RolesController(IRoleService service)
        {
            _service = service;
        }

        public IActionResult List()
        {
            var result = _service.GetAllPagedFiltersAsync();
            return View(result.Data);
        }
        [HttpPost]
        public async Task<IActionResult> GetAllPagedAsync(GetFiltersRequest request)
        {
            var result = await _service.GetAllPagedAsync(new BaseDataTableRequest(Request), request);
            Message(result);
            return Json(result);
        }

        public async Task<IActionResult> Create()
        {
            var result = await _service.CreateAsync();
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdateRequest request)
        {
            var result = await _service.CreateAsync(request);
            Message(result);
            if (!result.IsSuccess)
            {
                return View(result.Data);
            }
            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Update(int id)
        {
            var result = await _service.UpdateAsync(id);
            if (!result.IsSuccess)
            {
                Message(result);
                return RedirectToAction(nameof(List));
            }
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CreateUpdateRequest request)
        {
            var result = await _service.UpdateAsync(request);
            Message(result);
            if (!result.IsSuccess)
            {
                return View(result.Data);
            }
            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return Json(result);
        }
    }
}