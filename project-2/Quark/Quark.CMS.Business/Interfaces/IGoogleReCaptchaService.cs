using AvvaMobile.Core.Business;

namespace Quark.CMS.Business.Interfaces;

public interface IGoogleReCaptchaService
{
    Task<ServiceResult> VerifyToken(string token);

}