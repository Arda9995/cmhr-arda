using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.API.Business.Models.Requests.AccountRequests
{
    public class SmsVerificationRequest
    {
        public string Email { get; set; }
        public string EmailCode { get; set; }
    }
    public class SmsVerificationRequestValidator : AbstractValidator<SmsVerificationRequest>
    {
        public SmsVerificationRequestValidator(IStringLocalizer<CommonResource> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .EmailAddress().WithMessage(x => localizer["ValidationForInvalidEmail"])
                .MaximumLength(150).WithMessage(x => localizer["ValidationForMaxLength"]);

            RuleFor(x => x.EmailCode)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .Length(6).WithMessage(x => localizer["ValidationForInvalidCodeLength"])
                .Matches("^[0-9]*$").WithMessage(x => localizer["ValidationForInvalidCodeFormat"]);
        }
    }
}