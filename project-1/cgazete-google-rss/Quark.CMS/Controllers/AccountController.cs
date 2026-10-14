using AvvaMobile.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Quark.Business.Common.Caching;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.AccountRequests;
using Quark.CMS.Business.Models.Responses.AccountResponses;

namespace Quark.CMS.Controllers
{
    public class AccountController(
        IAccountService service,
        IGoogleReCaptchaService googleReCaptchaService,
        AppSettingsKeys appSettingsKeys)
        : BaseController
    {
        [AllowAnonymous]
        public IActionResult SignIn()
        {
            var captchaSiteKey = appSettingsKeys.ReCaptcha_SiteKey;

            var request = new SignInRequest
            {
                ReCaptchaSiteKey = captchaSiteKey
            };

            return View("SignIn2", request);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            var captchaResponse = await googleReCaptchaService.VerifyToken(request.ReCaptchaToken);

            if (!captchaResponse.IsSuccess && appSettingsKeys.Use_Recaptcha)
            {
                Message(captchaResponse);

                return RedirectToAction("SignIn");
            }

            var response = await service.SignInAsync(request);

            Message(response);

            if (response.IsSuccess)
            {
                return RedirectToAction(nameof(SignInOTP), response.Data);
            }
            else
            {
                return View("SignIn2", request);
            }

        }

        [AllowAnonymous]
        public IActionResult SignInOTP(SignInResponse response)
        {
            var request = new SignInOTPRequest
            {
                SignInToken = response.SignInToken
            };

            return View("SignInOTP2", request);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignInOTP(SignInOTPRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View("SignInOTP2", request);
            }

            var response = await service.SignInOTPAsync(request);

            Message(response);

            if (!response.IsSuccess)
            {
                return RedirectToAction("SignIn");
            }

            return Redirect("/");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Sign_Out()
        {
            await service.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }

        public async Task<IActionResult> MyProfile()
        {
            var result = await service.GetProfile();
            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var result = await service.UpdateProfile(request);
            Message(result);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Forbidden()
        {
            return View();
        }        
     
        public IActionResult ChangeLanguage(string lang)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return Redirect("/");
        }

    }
}