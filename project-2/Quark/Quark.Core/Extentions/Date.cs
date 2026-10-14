using Microsoft.Extensions.Localization;
using Quark.Resources;
using System.Globalization;

namespace Quark.Core.Extensions
{
    public static class Date
    {
        public static IStringLocalizer<CommonResource> localizer;
        public static string CalculateElapsedTime(this DateTime start_Time)
        {
            var end_time = DateTime.Now;
            var subtractedDate = end_time.Subtract(start_Time);
            string result;
            if (subtractedDate.Days >= 7)
            {
                var weeks = (int)(subtractedDate.Days / 7);
                if (weeks >= 52)
                {
                    var years = (int)(weeks / 52);
                    result = years + localizer["YearAgo"];
                }
                else
                {
                    result = weeks + localizer["WeeksAgo"];
                }

            }
            else if (subtractedDate.Days > 0 && subtractedDate.Days < 7)
            {
                result = subtractedDate.Days + localizer["DaysAgo"];
            }
            else
            {
                if (subtractedDate.Hours > 0)
                {
                    result = subtractedDate.Hours + localizer["HoursAgo"];
                }
                else
                {
                    if (subtractedDate.Minutes > 0)
                    {
                        result = subtractedDate.Minutes + localizer["MinsAgo"];
                    }
                    else
                    {
                        result = localizer["MinAgo"];
                    }
                }
            }
            return result;
        }
        public static string GetGreetingMessage(this DateTime timeOfDayGreeting)
        {
            var greetingMessage = "";

            if (timeOfDayGreeting.Hour >= 5 && timeOfDayGreeting.Hour < 12)
            {
                greetingMessage = localizer["GoodMorning"];
            }
            else if (timeOfDayGreeting.Hour >= 12 && timeOfDayGreeting.Hour < 16)
            {
                greetingMessage = localizer["GoodAfternoon"];
            }
            else if (timeOfDayGreeting.Hour >= 16 && timeOfDayGreeting.Hour < 20)
            {
                greetingMessage = localizer["GoodEvening"];
            }
            else
            {
                greetingMessage = localizer["GoodNight"];
            }
            return greetingMessage;
        }

		/// <summary>
		/// TimeSpan'i özel bir formata çevirir. (Örnek: 7g. 3s. 2d. 10sn.)
		/// </summary>
		/// <param name="timeSpan">Fark alınan TimeSpan</param>
		/// <param name="emptyMessage">Zaman dolduğunda gösterilecek mesaj (Varsayılan: "Süre doldu!")</param>
		/// <returns>Formatlanmış zaman string</returns>
		public static string DateToShortCountdown(this DateTime endDate, DateTime startDate)
		{
            var timeSpan = endDate - startDate;
			if (timeSpan.TotalSeconds <= 0)
				return "";

			var timeParts = new List<string>();

			if (timeSpan.Days > 0)
				timeParts.Add($"{timeSpan.Days}g.");
			if (timeSpan.Hours > 0)
				timeParts.Add($"{timeSpan.Hours}s.");
			if (timeSpan.Minutes > 0)
				timeParts.Add($"{timeSpan.Minutes}d.");
			if (timeSpan.Seconds > 0)
				timeParts.Add($"{timeSpan.Seconds}sn.");

			return string.Join(" ", timeParts);
		}

		public static string DateToLongCountdown(this DateTime endDate, DateTime startDate)
		{
			var timeSpan = endDate - startDate;
			if (timeSpan.TotalSeconds <= 0)
				return "";

			var timeParts = new List<string>();

			if (timeSpan.Days > 0)
				timeParts.Add($"{timeSpan.Days} gün,");
			if (timeSpan.Hours > 0)
				timeParts.Add($"{timeSpan.Hours} saat,");
			if (timeSpan.Minutes > 0)
				timeParts.Add($"{timeSpan.Minutes} dakika,");
			if (timeSpan.Seconds > 0)
				timeParts.Add($"{timeSpan.Seconds} saniye");

			return string.Join(" ", timeParts);
		}
        public static string ToTurkishDateFormat(this DateTime date)
        {
            return date.ToString("d MMMM yyyy", new CultureInfo("tr-TR"));
        }
    }
}