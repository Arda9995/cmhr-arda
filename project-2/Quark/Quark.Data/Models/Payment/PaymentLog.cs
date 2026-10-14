using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Payment;

[Table("Payment.PaymentLogs")]
public class PaymentLog
{
    public int ID { get; set; }
    public DateTime CreatedDate { get; set; }
    public long PaymentProviderSystemTime { get; set; }
    public string Message { get; set; }
    public string PaymentReferenceCode { get; set; }
    public string ErrorGroup { get; set; }
    public string ErrorCode { get; set; }
    public int StatusCode { get; set; }
    public string ConversationID { get; set; }
    public string Status { get; set; }
}