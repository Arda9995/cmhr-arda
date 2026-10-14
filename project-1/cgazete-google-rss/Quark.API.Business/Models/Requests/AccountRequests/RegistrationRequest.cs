using FluentValidation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Quark.Data.Models;
using Quark.Resources;

namespace Quark.API.Business.Models.Requests.AccountRequests
{
    public class RegistrationRequest
    {
        public int UserID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FullName { get; set; }
        public string MobilePhone { get; set; }

        public string Address { get; set; }
        public string IdentityNumber { get; set; }
        public string CompanyIdentityNumber { get; set; }
        public bool IsForeignCitizen { get; set; }
        
        // Corporate Billing Info
        public string BillingType { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string TaxOffice { get; set; }
        
        // Personal Info
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public string Gender { get; set; } 
        
        public int SelectedCountryID { get; set; }
        public int SelectedDisctrictID { get; set; }
        public int SelectedCityID { get; set; }

        public int SelectedCompanyCountryID { get; set; }
        public int SelectedCompanyDisctrictID { get; set; }
        public int SelectedCompanyCityID { get; set; }

        public List<SelectListItem> Countries { get; set; }

        public int TypeID { get; set; }
    }
    
    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationRequestValidator(IStringLocalizer<CommonResource> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .EmailAddress().WithMessage(x => localizer["ValidationForInvalidEmail"])
                .MaximumLength(150).WithMessage(x => localizer["ValidationForMaxLength"]);
                
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .MinimumLength(8).WithMessage(x => localizer["ValidationForMinLength"])
                .Matches("[A-Z]").WithMessage(x => localizer["ValidationForPasswordUppercase"])
                .Matches("[a-z]").WithMessage(x => localizer["ValidationForPasswordLowercase"])
                .Matches("[0-9]").WithMessage(x => localizer["ValidationForPasswordDigit"])
                .Matches("[^a-zA-Z0-9]").WithMessage(x => localizer["ValidationForPasswordSpecialChar"]);
                
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .Equal(x => x.Password).WithMessage(x => localizer["ValidationForPasswordMismatch"]);

            RuleFor(x => x.Firstname)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(x => localizer["ValidationForMaxLength"]);

            RuleFor(x => x.Lastname)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .MaximumLength(50).WithMessage(x => localizer["ValidationForMaxLength"]);

            RuleFor(x => x.MobilePhone)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .Matches(@"^(\+90|0)?[0-9]{10}$").WithMessage(x => localizer["ValidationForInvalidPhone"]);

            RuleFor(x => x.IdentityNumber)
                .NotEmpty().Unless(x => x.IsForeignCitizen)
                .WithMessage(x => localizer["ValidationForRequired"])
                .Length(11).Unless(x => x.IsForeignCitizen)
                .WithMessage(x => localizer["ValidationForTCLength"]);

            // Corporate validation rules
            When(x => x.TypeID == (int)MemberTypeEnum.Corporate, () =>
            {
                RuleFor(x => x.CompanyName)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                    .MaximumLength(100).WithMessage(x => localizer["ValidationForMaxLength"]);

                RuleFor(x => x.CompanyAddress)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                    .MaximumLength(500).WithMessage(x => localizer["ValidationForMaxLength"]);

                RuleFor(x => x.CompanyPhone)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                    .Matches(@"^(\+90|0)?[0-9]{10}$").WithMessage(x => localizer["ValidationForInvalidPhone"]);

                RuleFor(x => x.TaxOffice)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                    .MaximumLength(100).WithMessage(x => localizer["ValidationForMaxLength"]);

                RuleFor(x => x.CompanyIdentityNumber)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                    .Length(10).WithMessage(x => localizer["ValidationForTaxNumberLength"]);
            });

            // Location validation rules
            RuleFor(x => x.SelectedCountryID)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);

            RuleFor(x => x.SelectedCityID)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);

            RuleFor(x => x.SelectedDisctrictID)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);

            // Personal info validation rules
            RuleFor(x => x.BirthDay)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .InclusiveBetween(1, 31).WithMessage(x => localizer["ValidationForInvalidDate"]);
            
            RuleFor(x => x.BirthMonth)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .InclusiveBetween(1, 12).WithMessage(x => localizer["ValidationForInvalidDate"]);
            
            RuleFor(x => x.BirthYear)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .InclusiveBetween(1900, 2026).WithMessage(x => localizer["ValidationForInvalidDate"]);
            
            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .Must(x => x == "kadin" || x == "erkek").WithMessage(x => localizer["ValidationForInvalidGender"]);
        }
    }
} 