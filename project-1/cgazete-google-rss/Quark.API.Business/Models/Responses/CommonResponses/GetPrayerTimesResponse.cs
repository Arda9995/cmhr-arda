using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.API.Business.Models.Responses.NewsResponses;

namespace Quark.API.Business.Models.Responses.CommonResponses
{
	public class GetPrayerTimesResponse
	{
        public string CityName { get; set; }
        public string Slug { get; set; }
        public string Date { get; set; }
        public List<CategoryNews> CategoryNews { get; set; } = new List<CategoryNews>();
		public List<PrayerTime> PrayerTimes { get; set; } = new List<PrayerTime>();

		public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
    }
	public class PrayerTime
	{
		public string Date { get; set; }
		public string Fajr { get; set; }
		public string Sunrise { get; set; }
		public string Dhuhr { get; set; }
		public string Asr { get; set; }
		public string Maghrib { get; set; }
		public string Isha { get; set; }
	}
}
