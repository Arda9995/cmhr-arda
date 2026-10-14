namespace Quark.Business.Common.PaymentServices.Models.Responses;

public class CreateCumhuriyetSubscriptionResponse
{
    public string SubscriptionReferenceCode { get; set; }
    public string SubscriptionParentReferenceCode { get; set; }
    public string ConversationId { get; set; }
    public string SubscriptionStatus { get; set; }
    public long? SubscriptionStartDate { get; set; }
    public string CustomerReferenceCode { get; set; }
    public string SubscriptionPlanReferenceCode { get; set; }
}