namespace Quark.CMS.Business.Models.Responses.PaymentManagementResponses;

public class ListResponseSubscriptionItem
{
    public int ID { get; set; }
    public string MemberName { get; set; }
    public int MemberID { get; set; }
    public string SubscriptionReferenceCode { get; set; }
    public string Price { get; set; }
    public string SubscriptionStatus { get; set; }
    public string SubscriptionStatuClassName { get; set; }
    public string SubscriptionStartDate { get; set; }
    public string SubscriptionEndDate { get; set; }
    public string SubscriptionPlanReferenceCode { get; set; }
}