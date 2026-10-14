using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data;

[Table("Common.Genders")]
public class Gender
{
    public int ID { get; set; }
    public int LanguageID { get; set; }
    public string Name { get; set; }
}