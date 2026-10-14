using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.UserContracts;

[Table("UserContracts")]
public class UserContract
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}