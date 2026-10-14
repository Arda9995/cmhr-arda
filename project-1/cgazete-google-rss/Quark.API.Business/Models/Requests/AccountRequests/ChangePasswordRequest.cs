using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Quark.API.Business.Models.Requests.AccountRequests
{
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator(IStringLocalizer<ChangePasswordRequest> localizer)
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .MinimumLength(6).WithMessage(x => localizer["ValidationForMinLength"]);

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