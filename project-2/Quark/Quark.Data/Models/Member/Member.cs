using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Member.Members")]
public class Member
{
	public int ID { get; set; }
	public string Firstname { get; set; }
	public string Lastname { get; set; }
	public string Email { get; set; }
	public string MobilePhone { get; set; }
	public string CompanyPhone { get; set; }
	public string ProfilePhotoUrl { get; set; }
	public string MobileDevicePushID { get; set; }
	public bool EmailSmsCode { get; set; }
	public string EmailCode { get; set; }
	public string Password { get; set; }
	public DateTime? EmailCodeExpireDate { get; set; }
	public string RefreshToken { get; set; }
	public DateTime? RefreshTokenExpireDate { get; set; }
	public DateTime? PremiumMembershipExpirationDate { get; set; }
	public DateTime? LastLoginDate { get; set; }
	public DateTime CreatedDate { get; set; } = DateTime.Now;
	public bool HasPremiumMembership { get; set; }
	public bool IsEnabled { get; set; } = true;
	public bool IsDeleted { get; set; } = false;
	public bool MarketingConsent { get; set; } = false;
	public DateTime? BirthDate { get; set; }
	public string Gender { get; set; }
	public DateTime RegistrationDate { get; set; }
	public string IdentityNumber { get; set; }
	public string CompanyIdentityNumber { get; set; }
	public bool IsNonTurkish { get; set; }
	public string CompanyName { get; set; }
	public string TaxOffice { get; set; }
	public string Address { get; set; }
	public string CompanyAddress { get; set; }
	public int? CountryID { get; set; }
	public int? CityID { get; set; }
	public int? DistrictID { get; set; }
	public int? CompanyCountryID { get; set; }
	public int? CompanyCityID { get; set; }
	public int? CompanyDistrictID { get; set; }
	public int? TypeID { get; set; }
	public string IyzicoCustomerReferenceCode { get; set; }
	public string BillingFullName { get; set; }
	public string ZipCode { get; set; }
	public string CompanyZipCode { get; set; }

	[NotMapped]
	public string FullName { get { return $"{Firstname} {Lastname}"; } }
}