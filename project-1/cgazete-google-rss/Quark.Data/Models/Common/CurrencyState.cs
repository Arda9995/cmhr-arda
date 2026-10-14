using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models
{
    [Table("Common.CurrencyStates")]
    public class CurrencyState
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public enum CurrencyStateEnum
    {
        Equal = 1,
        Increased = 2,
        Decreased = 3
    }
}