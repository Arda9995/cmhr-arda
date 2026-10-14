using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.Tags")]
public class Tag
{
    public int ID { get; set; }
    public string Text { get; set; }
    public string Url { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string Breadcrumb { get; set; }
    public string PhotoUrl { get; set; }
    public DateTime? CreatedDate { get; set; }
}