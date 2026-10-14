using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.AI;

[Table("AI.Prompts")]
public class Prompt
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }
    public int PromptTypeID { get; set; }
    public int RequestTypeID { get; set; }
}