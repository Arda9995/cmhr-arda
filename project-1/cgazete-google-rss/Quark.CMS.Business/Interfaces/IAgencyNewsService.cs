using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.AgencyNewsRequests;
using Quark.CMS.Business.Models.Responses.AgencyNewsResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface IAgencyNewsService
    {
        Task<ServiceResult<ListFiltersRequest>> GetAllPagedFilters();
        Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPaged(BaseDataTableRequest tableRequest, ListFiltersRequest request);
        Task<ServiceResult<AgencyNewsDetailResponse>> Detail(int agencyNewsID);
    }
}
