using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.NewsTags")]
public class NewsTag
{
    public long NewsID { get; set; }
    public int TagID { get; set; }
}