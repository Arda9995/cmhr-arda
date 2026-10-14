using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.CMS.Business.Models.Requests.UserRequests
{
    public class CreateRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Password { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public int SelectedRoleID { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public IFormFile ProfilePhoto { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

    }
    public class CreateRequestValidator : AbstractValidator<CreateRequest>
    {
        public CreateRequestValidator(IStringLocalizer<CommonResource> _localizer)
        {
            RuleFor(x => x.Firstname)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(100).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Lastname)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(100).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Password)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .MaximumLength(200).WithMessage(x => _localizer["ValidationForMaxLength"]);
            
            RuleFor(x => x.Email)
                .EmailAddress().NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .EmailAddress().MaximumLength(150).WithMessage(x => _localizer["ValidationForMaxLength"]);
          
            RuleFor(x => x.MobilePhone)
                .NotNull().WithMessage(x => _localizer["ValidationForRequired"])
                .Matches(@"^\+90\s?\(?[5][0-9]{2}\)?\s?[0-9]{3}\s?[0-9]{2}\s?[0-9]{2}$")
                .WithMessage(x => _localizer["ValidationForPhone"])
                .MaximumLength(20).WithMessage(x => _localizer["ValidationForMaxLength"]);


            RuleFor(x => x.SelectedRoleID)
                .GreaterThanOrEqualTo(0).WithMessage(_localizer["ValidationForRequired"]);
            
        }
    }
}