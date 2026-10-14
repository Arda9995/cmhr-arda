using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark.Hangfire.Business.Models.Responses.CurrenciesResponses
{
    public class NosyCurrencyResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string MessageTR { get; set; }
        public long SystemTime { get; set; }
        public string Endpoint { get; set; }
        public int RowCount { get; set; }
        public int CreditUsed { get; set; }
        public List<ExchangeRate> Data { get; set; }
    }

    public class ExchangeRate
    {
        public string CurrencyCode { get; set; }
        public string BaseCurrencyCode { get; set; }
        public string Description { get; set; }
        public decimal Buy { get; set; }
        public decimal Sell { get; set; }
        public decimal ChangeRate { get; set; }
    }

}
