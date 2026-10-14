namespace Quark.CMS.Business.Models.Responses.PaymentManagementResponses;

public class ListResponsePaymentPlansItem
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Price { get; set; } 
    public string PaymentInterval { get; set; }
    public string PlanReferenceCode { get; set; }
    public string ProductReferenceCode { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsPopular { get; set; }
    public string IsEnabledText { get; set; }
    public string IsPopularText { get; set; }
    public int ActiveSubscriptionCount { get; set; } = 0;
}