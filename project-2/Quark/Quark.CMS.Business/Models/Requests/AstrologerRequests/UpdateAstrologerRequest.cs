using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.AstrologerRequests;

public class UpdateAstrologerRequest
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool ShowInHome { get; set; }
}

public class UpdateRequestValidator : AbstractValidator<UpdateAstrologerRequest>
{
    public UpdateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
            
        RuleFor(x => x.Name)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);
        
    }
}