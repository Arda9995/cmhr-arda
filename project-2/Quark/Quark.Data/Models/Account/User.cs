using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("User.Users")]
public class User
{
    public int ID { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string MobilePhone { get; set; }
    public string Password { get; set; }
    public string ProfilePhotoUrl { get; set; }
    public bool IsEnabled { get; set; }
    public int RoleID { get; set; }
    public bool IsPasswordConstant { get; set; }
    public string SignInToken { get; set; }
    public string AuthToken { get; set; }
    public virtual Role Role { get; set; }
    
    [NotMapped]
    public string FullName => $"{Firstname} {Lastname}";
}