using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.PromptRequests;
using Quark.CMS.Business.Models.Responses.PromptResponses;

namespace Quark.CMS.Business.Interfaces;

public interface IPromptService
{
    Task<ServiceResult<GetPromptFiltersRequest>> GetAllPagedFilters();
    Task<BaseDataTableResponse<List<ListResponsePromptItem>>> GetAllPaged(BaseDataTableRequest tableRequest, GetPromptFiltersRequest request);
    Task<ServiceResult<CreatePromptRequest>> Create();
    Task<ServiceResult<CreatePromptRequest>> Create(CreatePromptRequest request);
    Task<ServiceResult<UpdatePromptRequest>> Update(int id);
    Task<ServiceResult<UpdatePromptRequest>> Update(UpdatePromptRequest request);
    Task<ServiceResult> Delete(int id);

}