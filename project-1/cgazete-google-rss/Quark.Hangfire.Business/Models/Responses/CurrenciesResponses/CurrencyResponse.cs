using Newtonsoft.Json;

namespace Quark.Hangfire.Business.Models.Responses.CurrenciesResponses
{
    public class CurrencyResponse
    {
        public int TotalCount { get; set; }
        public List<CurrencyResponseItem> Items { get; set; }
    }

    public class CurrencyResponseItem
    {
        [JsonProperty("Tarih")]
        public string Date { get; set; }

        [JsonProperty("TP_DK_USD_A")]
        public string USD { get; set; }

        [JsonProperty("TP_DK_EUR_A")]
        public string EURO { get; set; }

        [JsonProperty("TP_DK_GBP_A")]
        public string GBP { get; set; }

        // [JsonProperty("TP.MK.KUL.YTL")]
        // public string GOLD { get; set; } 

        // public UNIXTIME UNIXTIME { get; set; }
    }

    public class UNIXTIME
    {
        [JsonProperty("$numberLong")]
        public string NumberLong { get; set; }
    }
}