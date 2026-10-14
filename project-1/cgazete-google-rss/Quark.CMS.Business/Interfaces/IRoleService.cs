using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.RoleRequests;
using Quark.CMS.Business.Models.Responses.RoleResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface IRoleService
    {
        ServiceResult<GetFiltersRequest> GetAllPagedFiltersAsync();

        Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, GetFiltersRequest request);
        
        Task<ServiceResult<CreateUpdateRequest>> CreateAsync();
        
        Task<ServiceResult<CreateUpdateRequest>> CreateAsync(CreateUpdateRequest request);
        
        Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(int id);
        
        Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(CreateUpdateRequest request);
        
        Task<ServiceResult> DeleteAsync(int id);
    }
}