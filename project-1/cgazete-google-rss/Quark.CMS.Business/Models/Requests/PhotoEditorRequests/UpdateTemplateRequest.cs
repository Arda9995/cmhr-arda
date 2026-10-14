using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.PhotoEditor;

public class UpdateTemplateRequest
{
    public int Id { get; set; } 
    public string Name{ get; set; }
    public string BackgroundColor { get; set; }
    public int Opacity { get; set; }
    public int FontSize { get; set; }
    public string TextColor { get; set; }
    public string Base64Image { get; set; }
}

public class UpdateTemplateRequestValidator : AbstractValidator<UpdateTemplateRequest>
{
    public UpdateTemplateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"])
            .MaximumLength(350).WithMessage(_localizer["ValidationForMaxLength"]);

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