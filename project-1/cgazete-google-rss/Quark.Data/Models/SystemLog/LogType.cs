using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.LogModels;

[Table("Logs.LogTypes")]
public class LogType
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
}

public enum LogTypesEnum
{
    LogIn = 1,
    News = 2,
    Tag = 3,
    Source = 6,
    Category = 4,
    Hangfire = 5
}