using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Requests.MemberRequests;
using Quark.Business.Common.PaymentServices.Models.Requests;

namespace Quark.API.Controllers;

public class MemberController(IMemberService memberService) : ApiPublicBaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateMemberRequest request)
    {
        var result = await memberService.CreateAsync(request);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateMemberRequest request)
    {
        var result = await memberService.UpdateAsync(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> UserInfo(string mobileDevicePushID)
    {
        var result = await memberService.GetAsync(mobileDevicePushID);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Notifications()
    {
        var result = await memberService.ListNotificationsAsync();
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var result = await memberService.DeleteAsync();
        return Ok(result);
    }

    [HttpPost]
    public IActionResult CreateSubscription(CreateCumhuriyetSubscriptionRequest createSubsRequest)
    {
        var result = memberService.CreateSubscriptionAsync(createSubsRequest);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> RetrySubscriptionPayment(string referenceCode)
    {
        var result = await memberService.RetrySubscriptionPayment(referenceCode);
        return Ok(result);
    }
    
    [HttpGet]
    public IActionResult SearchSubscriptionDetail(string referenceCode)
    {
        var result = memberService.SearchSubscriptionDetail(referenceCode);
        return Ok(result);
    }

}