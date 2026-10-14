using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.MenuRequests;

public class UpdateMenuRequest
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int? ParentID { get; set; }
    public string URL { get; set; }
    public bool OpenInNewWindow { get; set; }
    public int Order { get; set; }
    public bool IsEnabled { get; set; }
    public int? DetailsId { get; set; }
}
public class UpdateRequestValidator : AbstractValidator<UpdateMenuRequest>
{
    public UpdateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(200).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.OpenInNewWindow)
            .NotNull().WithMessage(x =>_localizer["ValidationForRequired"]);
        
        RuleFor(x => x.IsEnabled)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
        
        RuleFor(x => x.URL)
            .MaximumLength(500).WithMessage(x => _localizer["ValidationForMaxLength"]);
    }
}