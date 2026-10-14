using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.BookRequests;
using Quark.CMS.Business.Models.Responses.BookResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface IBookService
    {
        ServiceResult<GetBookFiltersRequest> GetAllPagedFiltersAsync();
        Task<BaseDataTableResponse<List<ListResponseBookItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, GetBookFiltersRequest request);
        Task<ServiceResult<CreateBookRequest>> Create();
        Task<ServiceResult<CreateBookRequest>> CreateAsync(CreateBookRequest request);
        Task<ServiceResult<UpdateBookRequest>> UpdateAsync(int id);
        Task<ServiceResult<UpdateBookRequest>> UpdateAsync(UpdateBookRequest request);
        Task<ServiceResult> DeleteAsync(int id);
        Task<ServiceResult<BookDetailsResponse>> DetailsAsync(int id);

    }
}
