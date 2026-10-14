using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("AppSettings.Groups")]
public class AppSettingsGroup
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }

    [NotMapped] public List<AppSettings> Keys { get; set; }
}