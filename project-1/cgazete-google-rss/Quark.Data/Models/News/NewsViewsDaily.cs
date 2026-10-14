using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.ViewsDaily")]
public class NewsViewsDaily
{
    public int ID { get; set; }
    public long NewsID { get; set; }
    public DateTime ViewsDate { get; set; }
    public int ViewCount { get; set; }
}