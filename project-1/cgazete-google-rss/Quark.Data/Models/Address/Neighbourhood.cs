using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models
{
    [Table("Address.Neighbourhoods")]
    public class Neighbourhood
    {
        public int ID { get; set; }
        public int DistrictID { get; set; }
        public string Name { get; set; }
    }
}