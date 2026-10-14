using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.News;

[Table("News.PrintingNews")]
public class PrintingNews
{
    public long ID { get; set; }
    public long? NewsID { get; set; }
    public int? CategoryID { get; set; }
    public string Title { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string Content { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int? CreatedBy { get; set; }
}