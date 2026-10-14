using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Anomaly;

[Table("Anomalies.AnomalyDataTypes")]
public class AnomalyDataType
{
    public int ID { get; set; }
    public string Endpoint { get; set; }
    public string Name { get; set; }
}

public enum AnomalyDataTypeEnum : int
{
    News = 1
}