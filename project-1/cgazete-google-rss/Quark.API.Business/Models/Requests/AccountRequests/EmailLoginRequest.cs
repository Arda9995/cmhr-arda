using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Quark.API.Business.Models.Requests.AccountRequests
{
    public class EmailLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class EmailLoginRequestValidator : AbstractValidator<EmailLoginRequest>
    {
        public EmailLoginRequestValidator(IStringLocalizer<EmailLoginRequest> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .EmailAddress().WithMessage(x => localizer["ValidationForInvalidEmail"])
                .MaximumLength(150).WithMessage(x => localizer["ValidationForMaxLength"]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .MinimumLength(6).WithMessage(x => localizer["ValidationForMinLength"]);
        }
    }
} 