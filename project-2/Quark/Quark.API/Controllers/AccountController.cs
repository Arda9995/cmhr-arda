using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Requests.AccountRequests;
using Quark.API.Business.Models.Responses.AccountResponses;

namespace Quark.API.Controllers;

[ApiController]
public class AccountController(IAccountService accountService) : ApiPublicBaseController
{
    [HttpPost]
    public IActionResult GetAnonymousToken(GetAnonymousTokenRequest model)
    {
        var result = accountService.GetAnonymousToken(model);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        var result = await accountService.LoginAsync(model);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegistrationRequest model)
    {
        var result = await accountService.AccountInformations(model);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> VerifySmsCode(SmsVerificationRequest model)
    {
        var result = await accountService.LoginEmailVerificationAsync(model);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> EmailLogin(EmailLoginRequest model)
    {
        var result = await accountService.EmailLoginAsync(model);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        var result = await accountService.ForgotPasswordAsync(model);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        var result = await accountService.ResetPasswordAsync(model);
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest model)
    {
        var result = await accountService.ChangePasswordAsync(model);
        return Ok(result);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAccountInformation()
    {
        var result = await accountService.GetAccountInformationAsync();
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateAccountInformation(AccountMainPageResponse model)
    {
        var result = await accountService.UpdateAccountInformationAsync(model);
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CancelPremiumMembershipRenewal()
    {
        var result = await accountService.CancelPremiumMembershipRenewalAsync();
        return Ok(result);
    }
}