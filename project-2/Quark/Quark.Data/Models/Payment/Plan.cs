using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Payment;

[Table("Payment.Plans")]
public class Plan
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string ConversationID { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; }
    public string PaymentInterval { get; set; }
    public string PaymentIntervalDisplayName { get; set; }
    public int? PaymentIntervalCount { get; set; }
    public int? RecurrenceCount { get; set; }
    public int UsageDay { get; set; }
    public string PlanPaymentType { get; set; }
    public string ProductReferenceCode { get; set; }
    public string PlanReferenceCode { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public int CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public int UpdatedBy { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public bool IsPopular { get; set; } = false;
    public bool IsPaymentRequired { get; set; } = true;
}

public enum Plans
{
    Standart,
}