using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Common
{
    [Table("Common.WeatherData")]
    public class WeatherData
    {
        public int ID { get; set; }
        public int CityID { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string IconURL { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Degree { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Night { get; set; }
        public string Humidity { get; set; }
    }
}