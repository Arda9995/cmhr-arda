using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.CMS.Business.Models.Requests.CommonRequests;
using Quark.CMS.Business.Models.Requests.TagRequests;
using Quark.CMS.Business.Models.Responses.CommonResponses;
using Quark.CMS.Business.Models.Responses.TagResponses;

namespace Quark.CMS.Business.Interfaces
{
	public interface ITagService
	{
		Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPaged(BaseDataTableRequest tableRequest);
		Task<ServiceResult<ExcelExportResponse>> DownloadExcelExportAsync(ExcelExportRequest request);
        ServiceResult<CreateTagRequest> Create();
		Task<ServiceResult<CreateTagRequest>> Create(CreateTagRequest request);
		Task<ServiceResult<UpdateTagRequest>> Update(long id);
		Task<ServiceResult<UpdateTagRequest>> Update(UpdateTagRequest request);
		Task<ServiceResult> Delete(long id);
		Task<ServiceResult<DetailsResponse>> Details(long id);
		Task<BaseDataTableResponse<List<DetailsTagsNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetTagFiltersRequest request);
		Task<ServiceResult<MergeTagRequest>> Merge();
		Task<ServiceResult<MergeTagRequest>> Merge(MergeTagRequest request);
		Task<ServiceResult<List<SelectListItem>>> TagSearch(string search);
		Task<ServiceResult<SelectListItem>> TagSearchByID(int id);
	}
}
