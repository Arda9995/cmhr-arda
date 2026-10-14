using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.SurveyRequests;

public class UpdateSurveyRequest
{
    public int ID { get; set; }
    public string PhotoUrl { get; set; }
    public string Question { get; set; }
    public string Option1 { get; set; }
    public string Option2 { get; set; }
    public string Option3 { get; set; }
    public string Option4 { get; set; }
    public IFormFile Photo { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CreatedBy { get; set; }
    public string Slug { get; set; }
}

public class UpdateRequestValidator : AbstractValidator<UpdateSurveyRequest>
{
    public UpdateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
            
        RuleFor(x => x.Question)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(200).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
        RuleFor(x => x.Option1)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
        RuleFor(x => x.Option2)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(50).WithMessage(x => _localizer["ValidationForMaxLength"]);
    }
}