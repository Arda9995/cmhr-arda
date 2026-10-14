using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Blocks.BlockItems")]
public class BlockItem
{
    public int ID { get; set; }
    public long NewsID { get; set; }
    public int BlockID { get; set; }
    public int Order { get; set; }
}