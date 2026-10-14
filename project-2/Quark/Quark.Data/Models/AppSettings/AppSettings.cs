using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("AppSettings.Keys")]
public class AppSettings
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
    public string Label { get; set; }
    public string Description { get; set; }
    public string Placeholder { get; set; }
    public int Order { get; set; }
    public int GroupID { get; set; }
    public string DataType { get; set; }
    public string PreviewImage { get; set; }
}