using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.Caricaturists")]
public class Caricaturist
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Headline { get; set; }
}