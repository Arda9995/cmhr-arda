using AvvaMobile.Core.Business; 
using AvvaMobile.Core.DataTable; 
using Microsoft.AspNetCore.Mvc.Rendering; 
using Quark.CMS.Business.Models.Requests.BlockRequests; 
using Quark.CMS.Business.Models.Responses.BlockResponses; 
using Quark.CMS.Business.Models.Responses.BlokResponses; 
using Quark.CMS.Business.Models.Responses.CommonResponses; 
 
namespace Quark.CMS.Business.Interfaces; 
 
public interface IBlockService 
{ 
    Task<ServiceResult<ListFiltersRequest>> GetAllPagedFilters(); 
    Task<BaseDataTableResponse<List<ListResponseBlokItem>>> GetAllPaged(BaseDataTableRequest tableRequest, ListFiltersRequest request); 
    Task<PagingResponse<ListBlockNewsResponseItem>> GetNewsPaged(GetBlockNewsFiltersRequest request); 
    Task<ServiceResult<List<SelectListItem>>> GetAllSelectList(int? pageId = null); 
    Task<ServiceResult<MinMaxValuesResponse>> GetMinMaxValues(int blockId); 
    Task<ServiceResult<List<ListBlockNewsResponseItem>>> GetNewsByBlockId(int blockId); 
    Task<ServiceResult<CreateUpdateRequest>> Create(); 
    Task<ServiceResult<CreateUpdateRequest>> Create(CreateUpdateRequest request); 
    Task<ServiceResult<CreateUpdateRequest>> Update(int id); 
    Task<ServiceResult<CreateUpdateRequest>> Update(CreateUpdateRequest request); 
    Task<ServiceResult> Delete(int id); 
    Task<ServiceResult> SaveBlockNews(CreateBlockNewsRequest request); 
}