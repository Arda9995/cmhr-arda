using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("User.Roles")]
public class Role
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsEnabled { get; set; }
}

public enum RolesEnum : int
{
    Manager = 1,
    Editor = 2
}