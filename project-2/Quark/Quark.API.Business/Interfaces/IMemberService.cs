using AvvaMobile.Core.Business;
using AvvaMobile.Core.Handlers.JwtToken;
using Iyzipay.Model.V2.Subscription;
using Quark.API.Business.Models.Requests.MemberRequests;
using Quark.API.Business.Models.Responses.MemberResponses;
using Quark.API.Business.Models.Responses.Notifications;
using Quark.Business.Common.PaymentServices.Models.Requests;

namespace Quark.API.Business.Interfaces
{
    public interface IMemberService
    {
        Task<ServiceResult<Token>> CreateAsync(CreateMemberRequest request);

        Task<ServiceResult<Token>> UpdateAsync(UpdateMemberRequest request);

        Task<ServiceResult<GetMemberResponse>> GetAsync(string mobileDevicePushID);

        Task<ServiceResult<List<ListNotificationsResponse>>> ListNotificationsAsync();

        Task<ServiceResult> DeleteAsync();
        Task<ServiceResult> CreateSubscriptionAsync(CreateCumhuriyetSubscriptionRequest createSubsRequest);
        Task<ServiceResult> RetrySubscriptionPayment(string referenceCode);
        ServiceResult<SubscriptionResource> SearchSubscriptionDetail(string referenceCode);
    }
}