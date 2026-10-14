using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Content.Pages")]
public class Page
{
    public int ID { get; set; }
    public string Key { get; set; }
    public string Keywords { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Slug { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string Breadcrumb { get; set; }
}