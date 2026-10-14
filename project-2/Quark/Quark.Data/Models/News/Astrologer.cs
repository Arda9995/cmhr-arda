using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.Astrologers")]
public class Astrologer
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool ShowInHome { get; set; }
    public bool IsDeleted { get; set; }
}