using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.AccountRequests
{
    public class UpdateProfileRequest
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobilePhone { get; set; }
        public string RoleName { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public IFormFile ProfilePhoto { get; set; }
    }
    public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Firstname)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Lastname)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(100).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Password)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(300).WithMessage(_localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Email)
                .EmailAddress().NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .EmailAddress().MaximumLength(150).WithMessage(_localizer["ValidationForMaxLength"]);
        }
    }
}