using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.BlockRequests;

public class CreateUpdateRequest
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Key { get; set; }
    public string Title { get; set; }
    public int MinNewsCount { get; set; }
    public int MaxNewsCount { get; set; }
    public bool IsEnabled { get; set; }
    public string BlockTypeID { get; set; }
    public int AutoSliderDelay { get; set; }   
    public List<int> SelectedUserIDs { get; set; }
    public List<SelectListItem> Users { get; set; }
    public List<SelectListItem> BlockTypes { get; set; }
    public int SelectedPageID { get; set; }
    public List<SelectListItem> Pages { get; set; }
    public List<SelectListItem> Categories { get; set; }
    public int? SelectedCategoryID { get; set; }
    public int? Order { get; set; }

}

public class CreateBlockRequestValidator : AbstractValidator<CreateUpdateRequest>
{
    public CreateBlockRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);

        RuleFor(x => x.SelectedUserIDs)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"]);

        RuleFor(x => x.SelectedPageID)
            .NotEmpty().WithMessage(_localizer["ValidationForRequired"]);
        
        RuleFor(x => x.SelectedPageID)
            .GreaterThanOrEqualTo(0).WithMessage(_localizer["ValidationForRequired"]);
    }
}