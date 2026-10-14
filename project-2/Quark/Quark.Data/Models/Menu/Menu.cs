using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Menu.Menus")]
public class Menu
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int? ParentID { get; set; }
    public string Key { get; set; }
    public string URL { get; set; }
    public bool OpenInNewWindow { get; set; }
    public int Order { get; set; }
    public bool IsEnabled { get; set; }
}