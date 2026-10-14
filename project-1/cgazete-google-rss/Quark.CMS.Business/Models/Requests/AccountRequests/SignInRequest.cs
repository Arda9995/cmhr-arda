using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.AccountRequests;

public class SignInRequest
{
    public string Credential { get; set; }
    public string ReCaptchaToken { get; set; }
    public string ReCaptchaSiteKey { get; set; }
}
public class SignInValidator : AbstractValidator<SignInRequest>
{
    public SignInValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Credential)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"]);
    }
}