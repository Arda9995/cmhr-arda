using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.API.Business.Models.Responses.NewsResponses;

namespace Quark.API.Business.Models.Responses.CommonResponses
{
	public class GetCurrencyPageResponse
	{
        public decimal BuyingValue { get; set; }
        public decimal SellingValue { get; set; }
        public int Value { get; set; }
        public int SelectedCurrencyID { get; set; }
        public string SelectedCurrencyName { get; set; }
        public string SelectedCurrencySlug { get; set; }
        public decimal? ChangePercent { get; set; }
        public List<GetCurrencyResponse> Currencies { get; set; }
        public List<SelectListItem> CurrencySelectListItems{ get; set; }

        public List<CategoryNews> CategoryNews { get; set; }
        public List<CategoryNews> CurrencyNews { get; set; }
    }
}
