using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Payment;

[Table("Payment.SubscriptionOrders")]
public class PaymentSubscriptionOrder
{
    public int ID { get; set; }
    public int SubscriptionID { get; set; }
    public string SubscriptionOrderReferenceCode { get; set; }
    public string Price { get; set; }
    public DateTime? StartPeriod { get; set; }
    public DateTime? EndPeriod { get; set; }
    public string OrderStatus { get; set; }
    public DateTime InsertedDate { get; set; } = DateTime.UtcNow;
}