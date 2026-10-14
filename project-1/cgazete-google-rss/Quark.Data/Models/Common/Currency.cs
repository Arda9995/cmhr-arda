using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;
[Table("Common.Currencies")]
public class Currency
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public string DisplayName { get; set; }

}

public enum CurrencyEnum
{
    USD = 1,
    EUR = 2,
    GBP = 3,
    GOLD = 4
}