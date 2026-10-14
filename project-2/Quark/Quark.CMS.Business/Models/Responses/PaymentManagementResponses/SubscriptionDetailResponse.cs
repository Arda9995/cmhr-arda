namespace Quark.CMS.Business.Models.Responses.PaymentManagementResponses;

public class SubscriptionDetailResponse
{
    public int MemberID { get; set; }
    public SubscriptionAddress SubscriptionAddress { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string IdentityNumber { get; set; }
    public string MemberName { get; set; }
    public string BillingPersonName { get; set; }
    public string IyzicoCustomerReferenceCode { get; set; }
    public List<SubscriptionDetailResponseItem> SubscriptionDetailResponseItems { get; set; }
}

public class SubscriptionAddress
{
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Country { get; set; }
    public string District { get; set; }
    public string TaxOffice { get; set; }
}

public class SubscriptionDetailResponseItem
{
    public int ID { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string PaymentPlanName { get; set; }
    public string PaymentPlanPrice { get; set; }
    public string SubscriptionReferenceCode { get; set; }
    public string PaymentPlanReferenceCode { get; set; }
    public string ProductReferenceCode { get; set; }
    public string SubscriptionStatu { get; set; }
}