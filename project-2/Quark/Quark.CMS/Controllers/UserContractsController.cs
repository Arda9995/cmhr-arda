using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.UserContractRequests;

namespace Quark.CMS.Controllers;

[Authorize]
public class UserContractsController : BaseController
{
    private readonly IUserContractService _service;

    public UserContractsController(IUserContractService service)
    {
        _service = service;
    }
    
    public IActionResult List()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> GetAllPaged()
    {
        var result = await _service.GetAllPaged(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);  
    }
    
    public IActionResult Create()
    {
        var result = _service.Create();
        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserContractRequest request)
    {
        var result = await _service.Create(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(request);
        }
        return RedirectToAction("List");
    }
    
    public async Task<IActionResult> Update(int id)
    {
        var result = await _service.Update(id);
        Message(result);
        if (!result.IsSuccess)
        {
            return RedirectToAction("List");
        }
        return View(result.Data);
    }
        
    [HttpPost]
    public async Task<IActionResult> Update(UpdateUserContractRequest request)
    {
        var result = await _service.Update(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(request);
        }
        return RedirectToAction("List");
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        return Json(result);
    }
}