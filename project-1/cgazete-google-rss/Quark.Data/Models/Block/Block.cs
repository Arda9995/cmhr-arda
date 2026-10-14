using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Blocks.Blocks")]
public class Block
{
    public int ID { get; set; }
    public int? CategoryID { get; set; }
    public string Name { get; set; }
    public string Key { get; set; }
    public string Title { get; set; }
    public bool IsEnabled { get; set; }
    public int MinNewsCount { get; set; }
    public int MaxNewsCount { get; set; }
    public int AutoSlideDelay { get; set; }
    public int BlockTypeID { get; set; }
    public int PageID { get; set; }
    public int? Order { get; set; }
}