using Iyzipay.Model.V2.Subscription;

namespace Quark.Business.Common.PaymentServices.Models.Requests;

public class UpgradeCumhuriyetSubscriptionRequest
{
    public string SubscriptionReferenceCode { get; set; }
    public bool ResetRecurrenceCount { get; set; } = false;
    public bool UseTrial { get; set; } = false;
    public string UpgradePeriod { get; set; } = SubscriptionUpgradePeriod.NOW.ToString();
    public string NewPricingPlanReferenceCode { get; set; }
}