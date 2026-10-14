using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.ViewsMinutely")]
public class NewsViewsMinutely
{
    public int ID { get; set; }
    public long NewsID { get; set; }
    public DateTime ViewsDate { get; set; }
    public int ViewCount { get; set; }
}