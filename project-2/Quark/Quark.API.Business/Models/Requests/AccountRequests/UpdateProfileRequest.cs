using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.API.Business.Models.Requests.AccountRequests
{
    public class UpdateProfileRequest
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
    }
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator(IStringLocalizer<CommonResource> localizer)
        {
            RuleFor(x => x.Firstname)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(x => localizer["ValidationForMaxLength"]);

            RuleFor(x => x.Lastname)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(x => localizer["ValidationForMaxLength"]);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .EmailAddress().WithMessage(x => localizer["ValidationForInvalidEmail"])
                .MaximumLength(150).WithMessage(x => localizer["ValidationForMaxLength"]);

            RuleFor(x => x.MobilePhone)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .Matches(@"^(\+90|0)?[0-9]{10}$").WithMessage(x => localizer["ValidationForInvalidPhone"]);
        }
    }
}