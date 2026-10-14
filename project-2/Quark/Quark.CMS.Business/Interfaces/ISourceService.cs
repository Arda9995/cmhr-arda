using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.CMS.Business.Models.Requests.SourceRequests;
using Quark.CMS.Business.Models.Responses.SourceResponses;

namespace Quark.CMS.Business.Interfaces;

public interface ISourceService
{
    Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPaged(BaseDataTableRequest tableRequest);
    ServiceResult<CreateSourceRequest> Create();
    Task<ServiceResult<CreateSourceRequest>> Create(CreateSourceRequest request);
    Task<ServiceResult<UpdateSourceRequest>> Update(int id);
    Task<ServiceResult<UpdateSourceRequest>> Update(UpdateSourceRequest request);
    Task<ServiceResult> Delete(int id);
    Task<ServiceResult<DetailsResponse>> Details(int id);
    Task<BaseDataTableResponse<List<DetailsNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetDetailsFiltersRequest request);
    Task<List<SelectListItem>> SearchSources(string search);
    Task<SelectListItem> SearchSourcesByID(int id);
    Task<ServiceResult<MergeSourceRequest>> Merge(MergeSourceRequest request);
    Task<ServiceResult<List<SelectListItem>>> SourceSearch(string search);
    Task<ServiceResult<MergeSourceRequest>> Merge();
}