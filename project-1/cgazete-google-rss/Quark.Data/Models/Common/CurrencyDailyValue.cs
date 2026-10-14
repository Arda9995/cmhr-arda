using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models
{
    [Table("Common.CurrencyDailyValues")]
    public class CurrencyDailyValue
    {
        public int ID { get; set; }
        public int CurrencyID { get; set; }
        public decimal Value { get; set; }
        public decimal BuyingValue { get; set; }
        public decimal SellingValue { get; set; }
        public DateTime Date { get; set; }
        public int StateID { get; set; }
    }
}