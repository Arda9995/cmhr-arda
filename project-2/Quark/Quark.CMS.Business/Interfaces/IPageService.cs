using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.CMS.Business.Models.Requests.PageRequests;
using Quark.CMS.Business.Models.Responses.PageResponses;


namespace Quark.CMS.Business.Interfaces
{
    public interface IPageService
    {
        Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest);
        
        Task<ServiceResult<List<SelectListItem>>> GetAllSelectList();

        ServiceResult<CreateRequest> Create();

        Task<ServiceResult<CreateRequest>> CreateAsync(CreateRequest request);

        Task<ServiceResult<UpdateRequest>> UpdateAsync(int id);

        Task<ServiceResult<UpdateRequest>> UpdateAsync(UpdateRequest request);

        Task<ServiceResult> DeleteAsync(int id);
    }
}
