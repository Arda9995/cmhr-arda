namespace Quark.API.Business.Models.Responses.AccountResponses
{
	public class SubscriptionRequest
	{
        public bool IsEnableForPayment { get; set; }
        public List<PlanResponse> Plans { get; set; }
		public int SelectedPlanID { get; set; }
		public string CardOwnerFullName { get; set; }
		public string CardNumber { get; set; }
		public string CardMonth { get; set; }
		public string CardYear { get; set; }
		public string CardCVV { get; set; }
	}
	public class PlanResponse
	{
        public string PlanName { get; set; }
        public int PlanID { get; set; }
		public decimal Price { get; set; }
        public bool IsPopular { get; set; }
        public string PaymentIntervalDisplayName { get; set; }
        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }
    }
}
