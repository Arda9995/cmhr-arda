using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("User.RoleClaims")]
public class RoleClaim
{
    public int ID { get; set; }
    public int RoleID { get; set; }
    public string Key { get; set; }
}