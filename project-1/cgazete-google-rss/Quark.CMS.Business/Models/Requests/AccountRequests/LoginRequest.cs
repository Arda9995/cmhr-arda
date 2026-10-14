using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.AccountRequests
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Email).EmailAddress().NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
            RuleFor(x => x.Password).NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
        }
    }
}