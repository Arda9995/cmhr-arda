using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.AnomalyRequests;
using Quark.CMS.Business.Models.Responses.AnomalyResponses;
using Quark.Data.Models.News;

namespace Quark.CMS.Business.Interfaces;

public interface IAnomalyService
{
    Task<ServiceResult<ListFiltersRequest>> GetAllPagedFilters();
    Task<BaseDataTableResponse<List<ListResponseAnomalyItem>>> GetAllPaged(BaseDataTableRequest tableRequest, ListFiltersRequest request);
    Task<ServiceResult<DetailsResponse>> Details(int id);
    Task<ServiceResult> MarkAsResolved(int id);
    Task<ServiceResult> CheckNewsAnomaly(News news);
}