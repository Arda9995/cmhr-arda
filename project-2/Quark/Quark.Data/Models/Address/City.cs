using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models
{
    [Table("Address.Cities")]
    public class City
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int CountryID { get; set; }
    }
}