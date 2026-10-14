using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.API.Business.Models.Requests.MemberRequests
{
    public class UpdateMemberRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
    }
    public class UpdateMemberRequestValidator : AbstractValidator<UpdateMemberRequest>
    {
        public UpdateMemberRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Firstname).NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
            RuleFor(x => x.Lastname).NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
            RuleFor(x => x.Email).EmailAddress().NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
            RuleFor(x => x.MobilePhone).NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
        }
    }
}