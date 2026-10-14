using AvvaMobile.Core.Business;
using Quark.API.Business.Models.Responses.AccountResponses;

namespace Quark.API.Business.Interfaces
{
	public interface ISubscriptionService
	{
		Task<ServiceResult<SubscriptionRequest>> GetSubscriptionInformations();
	}
}
