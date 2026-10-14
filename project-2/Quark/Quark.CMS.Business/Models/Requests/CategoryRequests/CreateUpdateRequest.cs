using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.CategoryRequests;

public class CreateUpdateRequest
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public int? ParentID { get; set; }
    public string Url { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string Breadcrumb { get; set; }
    public string BaseURL { get; set; }
    public string PhotoUrl { get; set; }
    public string BackgroundPhotoUrl { get; set; }
    public IFormFile Photo { get; set; }
    public IFormFile BackgroundPhoto { get; set; }
}

public class CreateRequestValidator : AbstractValidator<CreateUpdateRequest>
{
    public CreateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Name)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(250).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.Url)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.Breadcrumb)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.SeoTitle)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(500).WithMessage(_localizer["ValidationForMaxLength"]);

        RuleFor(x => x.SeoDescription)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
    }
}