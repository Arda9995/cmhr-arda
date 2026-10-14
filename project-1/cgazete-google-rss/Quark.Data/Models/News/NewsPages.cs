using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.News;

[Table("News.NewsPages")]
public class NewsPages
{
    public int ID { get; set; }
    public string Name { get; set; }
}