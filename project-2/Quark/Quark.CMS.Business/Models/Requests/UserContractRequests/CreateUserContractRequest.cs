using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.UserContractRequests;

public class CreateUserContractRequest
{
    public string Title { get; set; }
    public string Content { get; set; }
}

public class CreateUserContractRequestValidator : AbstractValidator<CreateUserContractRequest>
{
    public CreateUserContractRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"])
            .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"]);
        
    }
}