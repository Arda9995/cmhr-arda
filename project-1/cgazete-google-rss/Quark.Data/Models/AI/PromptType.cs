using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.AI;

[Table("AI.PromptTypes")]
public class PromptType
{
    public int ID { get; set; }
    public string Name { get; set; }
}

public enum PromptTypesEnum : int
{
    Title = 1,
    Subtitle = 2,
    Content = 3,
    Tags = 4
}