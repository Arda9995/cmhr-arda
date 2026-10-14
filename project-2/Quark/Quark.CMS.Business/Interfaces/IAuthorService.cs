using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.AuthorRequests;
using Quark.CMS.Business.Models.Requests.CommonRequests;
using Quark.CMS.Business.Models.Responses.AuthorResponses;
using Quark.CMS.Business.Models.Responses.CommonResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface IAuthorService
    {
        ServiceResult<GetFiltersRequest> GetAllPagedFiltersAsync();

        Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, GetFiltersRequest request);
        Task<ServiceResult<CreateUpdateRequest>> Create();
        Task<ServiceResult<CreateUpdateRequest>> CreateAsync(CreateUpdateRequest request);
        Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(int id);
        Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(CreateUpdateRequest request);
        Task<ServiceResult<DetailsAuthorRequest>> DetailsAsync(int id);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<ExcelExportResponse>> DownloadExcelExportAsync(ExcelExportRequest request);
        Task<BaseDataTableResponse<List<DetailResponseNewsItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, DetailsAuthorRequest request);

    }
}