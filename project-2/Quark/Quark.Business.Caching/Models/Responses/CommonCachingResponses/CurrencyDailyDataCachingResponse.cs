namespace Quark.Business.Caching.Models.Responses.CommonCachingResponses;

public class CurrencyDailyDataCachingResponse
{
    public int ID { get; set; }
    public string Slug { get; set; }
    public string Name { get; set; }
    public decimal BuyingValue { get; set; }
    public decimal SellingValue { get; set; }
    public DateTime Date { get; set; }
    public string DisplayName { get; set; }
    public int StateID { get; set; }
}