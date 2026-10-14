using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Quark.API.Business.Models.Requests.AccountRequests
{
    public class LoginRequest
    {
        public string MobilePhone { get; set; }
    }
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator(IStringLocalizer<LoginRequest> _localizer)
        {
            RuleFor(x => x.MobilePhone).NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
        }
    }
}