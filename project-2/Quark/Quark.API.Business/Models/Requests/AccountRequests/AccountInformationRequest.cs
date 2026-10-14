using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Quark.API.Business.Models.Requests.AccountRequests
{
    public class AccountInformationRequest
    {
        // Billing Info
        public string BillingType { get; set; } // "bireysel" or "kurumsal"
        
        // Individual Billing Info
        public string FullName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string TCIdentityNumber { get; set; }
        public bool IsForeignCitizen { get; set; }
        
        // Corporate Billing Info
        public string CompanyName { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyDistrict { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        
        // Personal Info
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public string Gender { get; set; } // "kadin" or "erkek"
    }

    public class AccountInformationRequestValidator : AbstractValidator<AccountInformationRequest>
    {
        public AccountInformationRequestValidator(IStringLocalizer<AccountInformationRequest> localizer)
        {
            RuleFor(x => x.BillingType)
                .NotEmpty().WithMessage(x => localizer["ValidationForRequired"])
                .Must(x => x == "bireysel" || x == "kurumsal")
                .WithMessage(x => localizer["ValidationForInvalidBillingType"]);
            
            // Individual billing validation rules
            When(x => x.BillingType == "bireysel", () =>
            {
                RuleFor(x => x.FullName)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.Country)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.City)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.District)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.Address)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.PhoneNumber)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.TCIdentityNumber)
                    .NotEmpty().Unless(x => x.IsForeignCitizen)
                    .WithMessage(x => localizer["ValidationForRequired"])
                    .Length(11).Unless(x => x.IsForeignCitizen)
                    .WithMessage(x => localizer["ValidationForTCLength"]);
            });
            
            // Corporate billing validation rules
            When(x => x.BillingType == "kurumsal", () =>
            {
                RuleFor(x => x.CompanyName)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.CompanyCountry)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.CompanyCity)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.CompanyDistrict)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.CompanyAddress)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.CompanyPhoneNumber)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.TaxOffice)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
                
                RuleFor(x => x.TaxNumber)
                    .NotEmpty().WithMessage(x => localizer["ValidationForRequired"]);
            });
            
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
                .Must(x => x == "kadin" || x == "erkek")
                .WithMessage(x => localizer["ValidationForInvalidGender"]);
        }
    }
} 