using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.AstrologerRequests;
using Quark.CMS.Business.Models.Responses.AstrologerResponses;

namespace Quark.CMS.Business.Interfaces;

public interface IAstrologerService
{
    Task<BaseDataTableResponse<List<ListResponseAstrologerItem>>> GetAllPaged(BaseDataTableRequest tableRequest);
    ServiceResult<CreateAstrologerRequest> Create();
    Task<ServiceResult<CreateAstrologerRequest>> Create(CreateAstrologerRequest request);
    Task<ServiceResult<UpdateAstrologerRequest>> Update(int id);
    Task<ServiceResult<UpdateAstrologerRequest>> Update(UpdateAstrologerRequest request);
    Task<ServiceResult<AstrologerDetailsResponse>> Details(int id);
    Task<BaseDataTableResponse<List<DetailsAstrologerNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetAstrologerFiltersRequest request);
    Task<ServiceResult> Delete(int id);
}