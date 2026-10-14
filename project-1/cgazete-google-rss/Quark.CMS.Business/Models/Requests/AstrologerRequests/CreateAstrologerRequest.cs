using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.AstrologerRequests;

public class CreateAstrologerRequest
{
    public string Name { get; set; }
    public bool ShowInHome { get; set; }
}

public class CreateRequestValidator : AbstractValidator<CreateAstrologerRequest>
{
    public CreateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
            
        RuleFor(x => x.Name)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);
        
    }
}