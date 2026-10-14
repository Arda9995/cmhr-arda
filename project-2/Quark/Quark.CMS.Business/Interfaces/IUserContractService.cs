using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.UserContractRequests;
using Quark.CMS.Business.Models.Responses.UserContractResponses;

namespace Quark.CMS.Business.Interfaces;

public interface IUserContractService
{
    Task<BaseDataTableResponse<List<ListResponseUserContractItem>>> GetAllPaged(BaseDataTableRequest tableRequest);

    ServiceResult<CreateUserContractRequest> Create();

    Task<ServiceResult> Create(CreateUserContractRequest request);

    Task<ServiceResult<UpdateUserContractRequest>> Update(int id);

    Task<ServiceResult> Update(UpdateUserContractRequest request);

    Task<ServiceResult> Delete(int id);

}