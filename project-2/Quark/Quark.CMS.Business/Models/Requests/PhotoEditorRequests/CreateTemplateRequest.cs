using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.PhotoEditor;

public class CreateTemplateRequest
{
    public string Name{ get; set; }
    public string BackgroundColor { get; set; } = "#FF0000";
    public int Opacity { get; set; } = 100;
    public int FontSize { get; set; } = 35;
    public string TextColor { get; set; } = "#FFFFFF";
}

public class CreateTemplateRequestValidator : AbstractValidator<CreateTemplateRequest>
{
    public CreateTemplateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"])
            .MaximumLength(350).WithMessage("ValidationForMaxLength");


        RuleFor(x => x.Opacity)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"])
            .LessThanOrEqualTo(0).WithMessage(_localizer["ValidationForMin"])
            .GreaterThanOrEqualTo(100).WithMessage(_localizer["ValidationForMax"]);

        RuleFor(x => x.TextColor)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.BackgroundColor)
            .MaximumLength(50).WithMessage(_localizer["ValidationForMaxLength"]);
    }
}