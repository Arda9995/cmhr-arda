using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.UserRequests;

namespace Quark.CMS.Controllers
{
    [Authorize(Roles = SecurityClaimKeys.USERS)]
    public class UsersController(IUserService service) : BaseController
    {
        public IActionResult List()
        {
            var result = service.GetAllPagedFiltersAsync();
            return View(result.Data);
        }
        [HttpPost]
        public async Task<IActionResult> GetAllPagedAsync(GetFiltersRequest request)
        {
            var result = await service.GetAllPagedAsync(new BaseDataTableRequest(Request), request);
            Message(result);
            return Json(result);
        }

        public async Task<IActionResult> Create()
        {
            var result = await service.CreateAsync();
            return View(result.Data);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CreateRequest request)
        {
            var result = await service.CreateAsync(request);
            Message(result);
            if (!result.IsSuccess)
            {
                return View(result.Data);
            }
            return RedirectToAction(nameof(List));
        }
        
        public async Task<IActionResult> Update(int id)
        {
            var result = await service.UpdateAsync(id);
            if (!result.IsSuccess)
            {
                Message(result);
                return RedirectToAction(nameof(List));
            }
            return View(result.Data);
        }
        
        [HttpPost]
        public async Task<IActionResult> Update(UpdateRequest request)
        {
            var result = await service.UpdateAsync(request);
            Message(result);
            if (!result.IsSuccess)
            {
                return View(result.Data);
            }
            return RedirectToAction(nameof(List));
        }
        
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);
            return Json(result);
        }
    }
}