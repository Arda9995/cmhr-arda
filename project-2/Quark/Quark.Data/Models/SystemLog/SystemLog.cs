using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.SystemLog;

[Table("Logs.Logs")]
public class SystemLog
{
    public int ID { get; set; }
    public int? UserID { get; set; }
    public string Credential { get; set; }
    public string IpAddress { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public int LogTypeID { get; set; }
    public long? ObjectID { get; set; }
}