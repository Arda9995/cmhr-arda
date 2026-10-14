using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models
{
    [Table("Address.Countries")]
    public class Country
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}