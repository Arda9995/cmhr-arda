using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Anomaly;

[Table("Anomalies.Anomalies")]
public class Anomaly
{
    public int ID { get; set; }
    public long DataID { get; set; }
    public int DataTypeID { get; set; }
    public int IssueTypeID { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsSolved { get; set; }
    public string Description { get; set; }
    public int CreatedBy { get; set; }
    
    [ForeignKey("CreatedBy")]
    public virtual User CreatedByUser { get; set; }
}