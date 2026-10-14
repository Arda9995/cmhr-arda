using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.PromptRequests;

public class UpdatePromptRequest
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }
    public int PromptTypeID { get; set; }
    public List<SelectListItem> PromptTypes { get; set; }
    public int RequestTypeID { get; set; }
    public List<SelectListItem> RequestTypes { get; set; }
}

public class UpdatePromptRequestValidator : AbstractValidator<UpdatePromptRequest>
{
    public UpdatePromptRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"])
            .MaximumLength(250).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.PromptTypeID)
            .GreaterThanOrEqualTo(0).WithMessage(_localizer["ValidationForRequired"]);
        
        RuleFor(x => x.RequestTypeID)
            .GreaterThanOrEqualTo(0).WithMessage(_localizer["ValidationForRequired"]);
    }
}