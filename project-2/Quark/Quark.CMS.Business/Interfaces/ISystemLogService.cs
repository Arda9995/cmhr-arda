using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.SystemLogRequests;
using Quark.CMS.Business.Models.Responses.SystemLogResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface ISystemLogService
    {
        Task<ServiceResult<ListFiltersRequest>> GetAllPagedFiltersAsync();
        Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, ListFiltersRequest request);
        Task<ServiceResult<DetailsResponse>> Details(int id);
    }
}