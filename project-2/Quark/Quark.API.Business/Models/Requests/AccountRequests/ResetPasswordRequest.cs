using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Quark.API.Business.Models.Requests.AccountRequests
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator(IStringLocalizer<ResetPasswordRequest> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .EmailAddress().WithMessage(x => localizer["ValidationForInvalidEmail"])
                .MaximumLength(150).WithMessage(x => localizer["ValidationForMaxLength"]);

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .MinimumLength(8).WithMessage(x => localizer["ValidationForMinLength"])
                .Matches("[A-Z]").WithMessage(x => localizer["ValidationForPasswordUppercase"])
                .Matches("[a-z]").WithMessage(x => localizer["ValidationForPasswordLowercase"])
                .Matches("[0-9]").WithMessage(x => localizer["ValidationForPasswordDigit"])
                .Matches("[^a-zA-Z0-9]").WithMessage(x => localizer["ValidationForPasswordSpecialChar"]);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .Equal(x => x.NewPassword).WithMessage(x => localizer["ValidationForPasswordMismatch"]);
        }
    }
} 