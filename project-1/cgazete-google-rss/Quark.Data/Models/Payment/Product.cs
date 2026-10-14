using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Payment;

[Table("Payment.Products")]
public class Product
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ReferenceCode { get; set; }
    public string ConversationID { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public int CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public int UpdatedBy { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
}