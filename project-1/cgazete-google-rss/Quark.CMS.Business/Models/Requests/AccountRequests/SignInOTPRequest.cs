using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.AccountRequests;

public class SignInOTPRequest
{
    public string MobilePhone { get; set; }
    public string SignInToken { get; set; }
    public string Password { get; set; }
}
public class SignInOTPValidator : AbstractValidator<SignInOTPRequest>
{
    public SignInOTPValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"]);
    }
}