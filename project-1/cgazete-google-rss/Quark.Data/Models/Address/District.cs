using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models
{
    [Table("Address.Districts")]
    public class District
    {
        public int ID { get; set; }
        public int CityID { get; set; }
        public string Name { get; set; }
    }
}