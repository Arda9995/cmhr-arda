namespace Quark.Hangfire.Business.Models.Responses.CryptoResponses;

public class CoinMarketCryptoResponse
{
    public Status status { get; set; }
    public List<Datum> data { get; set; }
}

public class Status
{
    public DateTime timestamp { get; set; }
    public int error_code { get; set; }
    public object error_message { get; set; }
    public int elapsed { get; set; }
    public int credit_count { get; set; }
    public object notice { get; set; }
    public int total_count { get; set; }
}

public class Datum
{
    public int id { get; set; }
    public string name { get; set; }
    public string symbol { get; set; }
    public string slug { get; set; }
    public int num_market_pairs { get; set; }
    public DateTime date_added { get; set; }
    public List<string> tags { get; set; }
    public DateTime last_updated { get; set; }
    public Quote quote { get; set; }
}

public class Quote
{
    public TRY TRY { get; set; }
}

public class TRY
{
    public double price { get; set; }
    public double volume_24h { get; set; }
    public double volume_change_24h { get; set; }
    public double percent_change_1h { get; set; }
    public double percent_change_24h { get; set; }
    public double percent_change_7d { get; set; }
    public double percent_change_30d { get; set; }
    public double percent_change_60d { get; set; }
    public double percent_change_90d { get; set; }
    public DateTime last_updated { get; set; }
}