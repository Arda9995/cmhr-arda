using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.API.Business.Models.Responses.AccountResponses
{
    public class AccountMainPageResponse
    {
        // Personal Info
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string BirthDay { get; set; }
        public string BirthMonth { get; set; }
        public string BirthYear { get; set; }
        public DateTime? BirthDate { get; set; }
        public int TypeID { get; set; }



		public int CountryID { get; set; }
        public int CityID { get; set; }
        public int DistrictID { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string TCIdentityNumber { get; set; }
        public bool IsForeignCitizen { get; set; }
        public bool MarketingConsent { get; set; }
        
        // Corporate Billing Info
        public string CompanyName { get; set; }
        public string CompanyIdentityNumber { get; set; }
        public int CompanyCountryID { get; set; }
        public int CompanyCityID { get; set; }
        public int CompanyDistrictID { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        

        public string Gender { get; set; }
        
        // Membership info
        public bool HasPremiumMembership { get; set; }
        public string BillingFullName { get; set; }
        public string ZipCode { get; set; }
        public string CompanyZipCode { get; set; }
        public DateTime? PremiumMembershipExpirationDate { get; set; }
        public int RemainingDays { get; set; }
        public PlanResponse ActivePlan { get; set; }
        public List<PlanResponse> Plans { get; set; }
        public List<SelectListItem> Cities { get; set; }
        public List<SelectListItem> District { get; set; }
    }
} 