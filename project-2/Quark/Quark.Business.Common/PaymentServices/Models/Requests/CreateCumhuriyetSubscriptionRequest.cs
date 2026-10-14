namespace Quark.Business.Common.PaymentServices.Models.Requests;

public class CreateCumhuriyetSubscriptionRequest
{
    public string ConversationId { get; set; }
    public string SubscriptionInitialStatus { get; set; }
    public int PlanID { get; set; }
    public bool RegisterConsumerCard { get; set; } = true;
    public string PaymentCardCVC { get; set; }
    public string PaymentCardNumber { get; set; }
    public string PaymentCardExpireYear { get; set; }
    public string PaymentCardExpireMonth { get; set; }
    public string PaymentCardHolderName { get; set; }
}