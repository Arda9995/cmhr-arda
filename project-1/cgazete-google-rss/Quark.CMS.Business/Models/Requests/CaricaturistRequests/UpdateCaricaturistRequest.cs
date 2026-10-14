using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.CaricaturistRequests;

public class UpdateCaricaturistRequest
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Headline { get; set; }
}

public class UpdateRequestValidator : AbstractValidator<UpdateCaricaturistRequest>
{
    public UpdateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
            
        RuleFor(x => x.Name)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(20).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
        RuleFor(x => x.Headline)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);
        
    }
}