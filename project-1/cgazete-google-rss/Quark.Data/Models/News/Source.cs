using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.Sources")]
public class Source
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string PhotoUrl { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string Breadcrumb { get; set; }
    public string Url { get; set; }
    public DateTime? CreatedDate { get; set; }
}