using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.AI;

[Table("AI.RequestTypes")]
public class RequestType
{
    public int ID { get; set; }
    public string Name { get; set; }
}