using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.API.Business.Models.Responses.NewsResponses;

namespace Quark.API.Business.Models.Responses.CommonResponses
{
    public class WeatherPageResponse
    {
        public List<CategoryNews> CategoryNews { get; set; }
        public List<Cities> Cities { get; set; }
        public List<SelectListItem> CitySelectListItems { get; set; }
        public Dictionary<string, List<WeatherDataResponse>> WeatherDataResponse { get; set; }
    }
    public class WeatherDetailPageResponse
    {
        public List<CategoryNews> CategoryNews { get; set; }
        public List<Cities> Cities { get; set; }
        public List<SelectListItem> CitySelectListItems { get; set; }
        public Dictionary<string, List<WeatherDataResponse>> WeatherDataResponse { get; set; }
    }

    public class Cities
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public int ID { get; set; }
    }
    public class WeatherDataResponse
    {
        public string City { get; set; }
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
