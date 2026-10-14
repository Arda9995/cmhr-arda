namespace Quark.Business.Common.PaymentServices.Models.Requests;

public class CancelCumhuriyetSubscriptionRequest
{
    public string SubscriptionReferenceCode { get; set; }
    public string ConversationId { get; set; }
}