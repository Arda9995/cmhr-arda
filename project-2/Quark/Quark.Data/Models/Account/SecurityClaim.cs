using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("User.SecurityClaims")]
[Keyless]
public class SecurityClaim
{
    public string Key { get; set; }
    public string GroupName { get; set; }
    public string Description { get; set; }
}