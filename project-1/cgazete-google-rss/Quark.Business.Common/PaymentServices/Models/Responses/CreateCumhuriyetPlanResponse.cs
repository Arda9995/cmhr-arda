namespace Quark.Business.Common.PaymentServices.Models.Responses;

public class CreateCumhuriyetPlanResponse
{
    public string PlanName { get; set; }
    public string CurrencyCode { get; set; }
    public string PaymentInterval { get; set; }
    public int? PaymentIntervalCount { get; set; }
    public string PlanPaymentType { get; set; }
    public string ReferenceCode { get; set; }
    public string ConversationID { get; set; }
    
}