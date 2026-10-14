using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.UserRequests;
using Quark.CMS.Business.Models.Responses.UserResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface IUserService
    {
        ServiceResult<GetFiltersRequest> GetAllPagedFiltersAsync();

        Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, GetFiltersRequest request);

        Task<ServiceResult<CreateRequest>> CreateAsync();
        
        Task<ServiceResult<CreateRequest>> CreateAsync(CreateRequest request);
        
        Task<ServiceResult<UpdateRequest>> UpdateAsync(int id);
        
        Task<ServiceResult<UpdateRequest>> UpdateAsync(UpdateRequest request);
        
        Task<ServiceResult> DeleteAsync(int id);
    }
}