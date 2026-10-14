using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Payment;

[Table("Payment.SubscriptionOrderPaymentAttempts")]
public class PaymentSubscriptionOrderPaymentAttempt
{
    public int ID { get; set; }
    public int SubscriptionOrderID { get; set; }
    public string ConversationId { get; set; }
    public string PaymentID { get; set; }
    public string PaymentStatus { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime InsertedDate { get; set; } = DateTime.UtcNow;
}