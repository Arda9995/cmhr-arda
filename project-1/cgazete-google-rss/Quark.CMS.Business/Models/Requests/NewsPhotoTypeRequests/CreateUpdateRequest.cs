using FluentValidation;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.NewsPhotoTypeRequests
{
    public class CreateUpdateRequest
    {
        public int ID { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsRequired { get; set; }
        public int Order { get; set; }
    }
    public class CreateRequestValidator : AbstractValidator<CreateUpdateRequest>
    {
        public CreateRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Key)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);

            RuleFor(x => x.Name)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(_localizer["ValidationForMaxLength"]);

            RuleFor(x => x.Description)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(200).WithMessage(_localizer["ValidationForMaxLength"]);

            RuleFor(x => x.Width)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"]);

            RuleFor(x => x.Height)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"]);

            RuleFor(x => x.Order)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
        }
    }
}
