using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.API.Business.Models.Requests.MemberRequests
{
    public class CreateMemberRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string MobilePhone { get; set; }
    }
    public class CreateMemberRequestValidator : AbstractValidator<CreateMemberRequest>
    {
        public CreateMemberRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Firstname).NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
            RuleFor(x => x.Lastname).NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
            RuleFor(x => x.MobilePhone).NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
        }
    }
}