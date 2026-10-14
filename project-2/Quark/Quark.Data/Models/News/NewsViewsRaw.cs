using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.NewsViewsRaw")]
public class NewsViewsRaw
{
    public int ID { get; set; }
    public long NewsID { get; set; }
}