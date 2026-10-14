using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.SourceRequests
{
    public class MergeSourceRequest
    {
        public List<SelectListItem> Sources { get; set; }
        public int SelectedTargetSourceID { get; set; }
        public int SelectedSourceSourceID { get; set; }
    }

    public class MergeSourceRequestValidator : AbstractValidator<MergeSourceRequest>
    {
        public MergeSourceRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Sources).NotNull().NotEmpty().WithMessage(_localizer["ValidationForRequired"]);

            RuleFor(x => x.SelectedTargetSourceID)
              .GreaterThan(0).WithMessage(_localizer["ValidationForRequired"]);

            RuleFor(x => x.SelectedSourceSourceID)
                .GreaterThan(0).WithMessage(_localizer["ValidationForRequired"]);
        }
    }
}
