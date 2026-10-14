using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("User.UserCategories")]
public class UserCategory
{
    public int UserID { get; set; }
    public int CategoryID { get; set; }
}