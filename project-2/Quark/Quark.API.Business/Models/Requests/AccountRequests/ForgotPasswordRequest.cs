using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Quark.API.Business.Models.Requests.AccountRequests
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }

    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordRequestValidator(IStringLocalizer<ForgotPasswordRequest> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .EmailAddress().WithMessage(x => localizer["ValidationForInvalidEmail"])
                .MaximumLength(150).WithMessage(x => localizer["ValidationForMaxLength"]);
        }
    }
} 