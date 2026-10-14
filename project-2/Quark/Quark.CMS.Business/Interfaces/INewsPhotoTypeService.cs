using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.CommonRequests;
using Quark.CMS.Business.Models.Requests.NewsPhotoTypeRequests;
using Quark.CMS.Business.Models.Responses.CommonResponses;
using Quark.CMS.Business.Models.Responses.NewsPhotoTypeResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface INewsPhotoTypeService
    {
        Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest);

        Task<ServiceResult<ExcelExportResponse>> DownloadExcelExportAsync(ExcelExportRequest request);

        ServiceResult<CreateUpdateRequest> Create();

        Task<ServiceResult<CreateUpdateRequest>> CreateAsync(CreateUpdateRequest request);

        Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(int id);

        Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(CreateUpdateRequest request);

        Task<ServiceResult> DeleteAsync(int id);
    }
}