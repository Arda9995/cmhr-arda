using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Blocks.BlockTypes")]
public class BlockType
{
    public int ID { get; set; }
    public string Name { get; set; }
}

public enum BlockTypesEnum : int
{
    Slider = 1,
    Catalog = 2,
}