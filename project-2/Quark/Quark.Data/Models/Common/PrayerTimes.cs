using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Common
{
    [Table("Common.PrayerTimes")]
    public class PrayerTimes
    {
        public int ID { get; set; }
        public int CityID { get; set; }
        public string Date { get; set; }
        public string Fajr { get; set; }
        public string Sunrise { get; set; }
        public string Dhuhr { get; set; }
        public string Asr { get; set; }
        public string Maghrib { get; set; }
        public string Isha { get; set; }
    }
}