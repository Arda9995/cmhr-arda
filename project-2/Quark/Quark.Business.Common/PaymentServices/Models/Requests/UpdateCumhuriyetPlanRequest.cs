namespace Quark.Business.Common.PaymentServices.Models.Requests;

public class UpdateCumhuriyetPlanRequest
{
    public int PlanID { get; set; }
    public string ConversationID { get; set; }
    public string Name { get; set; }
    public string PricingPlanReferenceCode { get; set; }
    public string TrialPeriodDays { get; set; }
    public bool IsEnabled { get; set; }
}