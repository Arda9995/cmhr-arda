using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.Business.Common.PaymentServices.Models.Requests;

public class CreateCumhuriyetPlanRequest
{
    public string ConversationId { get; set; }
    public string Name { get; set; }
    public string ProductReferenceCode { get; set; }
    public int? RecurrenceCount { get; set; }
    public string PlanPaymentType { get; set; }
    public int PaymentIntervalCount { get; set; } = 1;
    public string PaymentInterval { get; set; }
    public string PaymentIntervalDisplayName { get; set; }
    public string Currency { get; set; }
    public string Price { get; set; }
    public bool IsPopular { get; set; }
    public List<SelectListItem> ProductReferenceCodes { get; set; }
}