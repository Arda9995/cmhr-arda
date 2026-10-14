using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.SourceRequests;

public class CreateSourceRequest
{
    public string Name { get; set; }
    public string PhotoUrl { get; set; }
    public IFormFile Photo { get; set; }
    public string Url { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string Breadcrumb { get; set; }
    public string BaseUrl { get; set; }
}

public class CreateRequestValidator : AbstractValidator<CreateSourceRequest>
{
    public CreateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
            
        RuleFor(x => x.Name)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
        RuleFor(x => x.Url)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(100).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
        RuleFor(x => x.SeoTitle)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(500).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
        RuleFor(x => x.SeoDescription)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
            
        RuleFor(x => x.Breadcrumb)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(100).WithMessage(x => _localizer["ValidationForMaxLength"]);
    }
}