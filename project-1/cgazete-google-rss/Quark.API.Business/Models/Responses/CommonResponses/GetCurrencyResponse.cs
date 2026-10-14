namespace Quark.API.Business.Models.Responses.CommonResponses
{
	public class GetCurrencyResponse
	{
        public int ID { get; set; }
        public string Code { get; set; }
        public string Slug { get; set; }
        public string Name{ get; set; }
        public string DisplayName { get; set; }
    }
}
