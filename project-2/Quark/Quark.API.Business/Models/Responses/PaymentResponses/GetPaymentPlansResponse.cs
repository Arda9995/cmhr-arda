using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.API.Business.Models.Responses.PaymentResponses
{
	public class GetPaymentPlansResponse
	{
		public List<SelectListItem> PaymentPlans { get; set; }
	}
}
