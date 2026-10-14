using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.RssLinks")]
public class RssLink
{
    public int ID { get; set; }
    public string Link { get; set; }
    public int? CategoryID { get; set; }
    public int AgencyID { get; set; }
    public bool IsActive { get; set; }
}