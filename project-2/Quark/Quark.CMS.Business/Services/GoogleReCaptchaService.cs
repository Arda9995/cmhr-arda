using AvvaMobile.Core;
using AvvaMobile.Core.Business;
using Quark.Business.Common.Caching;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Responses.ReCaptchaResponses;

namespace Quark.CMS.Business.Services;

public class GoogleReCaptchaService(NetworkManager networkManager, AppSettingsKeys appSettingsKeys)
    : IGoogleReCaptchaService
{
    public async Task<ServiceResult> VerifyToken(string token)
    {
        var response = new ServiceResult();

        try
        {
            var secretKey = appSettingsKeys.ReCaptcha_SecretKey;
            var url = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}";

            var httpResponse = await networkManager.GetAsync<ReCaptchaResponse>(url);
            if (!httpResponse.IsSuccess)
            {
                response.IsSuccess = false;
                return response;
            }

            var responseData = httpResponse.Data;

            if (!responseData.Success)
            {               
                response.SetError( "Lütfen Captcha testini tamamlayınız.") ;
                return response;
            }

            return response;
        }
        catch (Exception)
        {
            response.IsSuccess = false;
            return response;
        }
    }
}