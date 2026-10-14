using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Common
{
    [Table("Common.CurrencyDailyData")]
    public class CurrencyDailyData
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public decimal? Buying { get; set; }
        public decimal? Selling { get; set; }
        public decimal? Latest { get; set; }
        public decimal? ChangeRate { get; set; }
        public decimal? DayMinimum { get; set; }
        public decimal? DayMaximum { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
