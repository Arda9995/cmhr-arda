using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.MemberRequests;

public class CreateUpdateRequest
{
    public int ID { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string MobilePhone { get; set; }
    public bool StaticSmsCode { get; set; }
    public string SmsCode { get; set; }
    public bool IsEnabled { get; set; }
    public string ProfilePhotoUrl { get; set; }
    public IFormFile ProfilePhoto { get; set; }

}
public class CreateRequestValidator : AbstractValidator<CreateUpdateRequest>
{
    public CreateRequestValidator(IStringLocalizer<CommonResource> _localizer)
    {
        RuleFor(x => x.Firstname)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);
        
        RuleFor(x => x.Lastname)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);;
        
        RuleFor(x => x.Email)
            .EmailAddress().NotNull().WithMessage(x => _localizer["ValidationForRequired"])
            .EmailAddress().MaximumLength(150).WithMessage(_localizer["ValidationForMaxLength"]);;
        
        RuleFor(x => x.MobilePhone)
            .NotNull().WithMessage(x => _localizer["ValidationForRequired"]);
        
        RuleFor(x => x.MobilePhone)
            .Matches("^\\+90\\s\\(\\d{3}\\)\\s\\d{3}\\s\\d{2}\\s\\d{2}$").WithMessage(x => _localizer["PhoneNumberIsNotValid"]);
    }
}