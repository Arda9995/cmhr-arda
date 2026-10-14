using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Anomaly;

[Table("Anomalies.AnomalyIssueTypes")]
public class AnomalyIssueType
{
    public int ID { get; set; }
    public string Name { get; set; }
}

public enum AnomalyIssueTypeEnum : int
{
    Date = 1,
    Link = 2
}