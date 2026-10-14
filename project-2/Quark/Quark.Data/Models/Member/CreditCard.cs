using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Member.CreditCards")]
public class CreditCard
{
    public int ID { get; set; }
    public int MemberID { get; set; }
    public string Name { get; set; }
    public string CardHolderName { get; set; }
    public string CardNo { get; set; }
    public string ExpirationMonth { get; set; }
    public string ExpirationYear { get; set; }
    public string CVV { get; set; }
    public DateTime CreatedDate { get; set; }
}