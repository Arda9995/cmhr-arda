namespace Quark.API.Business.Models.Responses.CommonResponses
{
    public class GetTodayCurrencyDataResponse
    {
        public List<TodayCurrencyDataItem> TodayCurrencyDataItems { get; set; } = new List<TodayCurrencyDataItem>();
    }
    public class TodayCurrencyDataItem
    {
        public string CurrencyName { get; set; }
        public string CurrencyValue { get; set; }
        public int Status { get; set; }
    }
}
