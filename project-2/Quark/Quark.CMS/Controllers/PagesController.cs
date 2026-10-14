using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Models.Requests.PageRequests;
using Quark.CMS.Business.Interfaces;

namespace Quark.CMS.Controllers
{
    [Authorize(Roles = SecurityClaimKeys.PAGES)]
    public class PagesController : BaseController
    {
        private readonly IPageService _service;

        public PagesController(IPageService service) 
        {
            _service = service;
        
        }
        
        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAllPagedAsync()
        {
            var result = await _service.GetAllPagedAsync(new BaseDataTableRequest(Request));
            Message(result);
            return Json(result);
        }

        public async Task<IActionResult> GetAllSelectList()
        {
            var result = await _service.GetAllSelectList();

            return Ok(result);
        }
        
        public IActionResult Create()
        {
            var result = _service.Create();
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRequest request)
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
        public async Task<IActionResult> Update(UpdateRequest request)
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