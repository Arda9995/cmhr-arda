using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.MemberRequests;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.MEMBERS)]
public class MembersController : BaseController
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    public IActionResult List()
    {
        var result = _memberService.GetAllPagedFiltersAsync();
        return View(result.Data);
    }
        
    [HttpPost]
    public async Task<IActionResult> GetAllPagedAsync(ListFiltersRequest request)
    {
        var result = await _memberService.GetAllPagedAsync(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }

    public IActionResult Create()
    {
        var model = new CreateUpdateRequest();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUpdateRequest request)
    {
        var result = await _memberService.CreateAsync(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return Redirect("/Members/Details/" + result.Data.ID);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _memberService.DetailsAsync(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction(nameof(List));
        }
        return View(result.Data);
    }

    public async Task<IActionResult> Update(int id)
    {
        var result = await _memberService.UpdateAsync(id);
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
        var result = await _memberService.UpdateAsync(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return Redirect("/Members/Details/" + request.ID);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _memberService.DeleteAsync(id);
        return Json(result);
    }

    public IActionResult SendPushNotification(int? id)
    {
        var model = new SendPushNotificationRequest
        {
            CustomerID = id
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SendPushNotification(SendPushNotificationRequest request)
    {
        var result = await _memberService.SendPushNotification(request);
        if (result.IsSuccess)
        {
            Message(result);
            return RedirectToAction(nameof(List));
        }
        else
        {
            return View(request);
        }
    }

    public async Task<IActionResult> GetNotificationsPaged(int id)
    {
        var result = await _memberService.GetNotificationsPagedAsync(new BaseDataTableRequest(Request), id);
        return Json(result);
    }

    public async Task<IActionResult> GetCreditCardsPaged(int id)
    {
        var result = await _memberService.GetCreditCardsPagedAsync(new BaseDataTableRequest(Request), id);
        return Json(result);
    }
}