using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.BlockLog
{
    [Table("Blocks.BlockLogs")]
    public class BlockLog
    {
        public int ID { get; set; }
        public int BlockID { get; set; }
        public long NewsID { get; set; }
        public string ChangeType { get; set; }
        public int UserID { get; set; }
        public int? OldOrder { get; set; }
        public int? NewOrder{ get; set; }
        public DateTime CreatedDate{ get; set; }
        public DateTime? PreviousChangeDate{ get; set; }
        public long? Duration { get; set; }
    }
}
