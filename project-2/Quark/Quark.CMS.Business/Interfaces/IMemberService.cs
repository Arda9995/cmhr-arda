using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.MemberRequests;
using Quark.CMS.Business.Models.Responses.MemberResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface IMemberService
    {
        ServiceResult<ListFiltersRequest> GetAllPagedFiltersAsync();
        
        Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, ListFiltersRequest request);
        
        Task<ServiceResult<CreateUpdateRequest>> CreateAsync(CreateUpdateRequest request);
        
        Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(int id);
        
        Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(CreateUpdateRequest request);
        
        Task<ServiceResult<DetailResponse>> DetailsAsync(int id);
        
        Task<ServiceResult> DeleteAsync(int id);
        
        Task<ServiceResult> SendPushNotification(SendPushNotificationRequest request);
        
        Task<BaseDataTableResponse<List<ListMemberNotificationsResponse>>> GetNotificationsPagedAsync(BaseDataTableRequest tableRequest, int id);
        
        Task<BaseDataTableResponse<List<ListMemberCreditCardsResponse>>> GetCreditCardsPagedAsync(BaseDataTableRequest tableRequest, int id);
    }
}