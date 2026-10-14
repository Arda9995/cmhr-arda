using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Blocks.BlockUsers")]
public class BlockUser
{
    public int ID { get; set; }
    public int BlockID { get; set; }
    public int UserID { get; set; }
}