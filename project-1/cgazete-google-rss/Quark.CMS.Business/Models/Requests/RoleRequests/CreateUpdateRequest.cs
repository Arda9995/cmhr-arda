using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.RoleRequests
{
    public class CreateUpdateRequest
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public List<SecurityClaimItem> SecurityClaims { get; set; }
    }
    public class SecurityClaimItem
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class CreateRequestValidator : AbstractValidator<CreateUpdateRequest>
    {
        public CreateRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Description)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(300).WithMessage(x => _localizer["ValidationForMaxLength"]);
        }
    }
}