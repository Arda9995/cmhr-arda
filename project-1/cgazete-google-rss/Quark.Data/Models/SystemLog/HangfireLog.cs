using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.SystemLog;

[Table("Logs.HangfireLogs")]
public class HangfireLog
{
    public int ID { get; set; }
    public string Method { get; set; }
    public string Message { get; set; }
    public DateTime CreatedDate { get; set; }
}