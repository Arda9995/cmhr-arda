using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.TagRequests;

public class MergeTagRequest
{
    public List<SelectListItem> Tags { get; set; } 
    public int SelectedTargetTagID { get; set; }
    public int SelectedSourceTagID { get; set; }
}

public class MergeTagRequestValidator : AbstractValidator<MergeTagRequest>
{
    public MergeTagRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x=>x.Tags).NotNull().NotEmpty().WithMessage(_localizer["ValidationForRequired"]);

        RuleFor(x => x.SelectedTargetTagID)
          .GreaterThan(0).WithMessage(_localizer["ValidationForRequired"]);

        RuleFor(x => x.SelectedSourceTagID)
            .GreaterThan(0).WithMessage(_localizer["ValidationForRequired"]);
    }
}