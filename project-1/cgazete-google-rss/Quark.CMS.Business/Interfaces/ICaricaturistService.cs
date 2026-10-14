using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.CaricaturistRequests;
using Quark.CMS.Business.Models.Responses.CaricaturistResponses;

namespace Quark.CMS.Business.Interfaces;

public interface ICaricaturistService
{
    Task<BaseDataTableResponse<List<ListResponseCaricaturistItem>>> GetAllPaged(BaseDataTableRequest tableRequest);
    ServiceResult<CreateCaricaturistRequest> Create();
    Task<ServiceResult<CreateCaricaturistRequest>> Create(CreateCaricaturistRequest request);
    Task<ServiceResult<UpdateCaricaturistRequest>> Update(int id);
    Task<ServiceResult<UpdateCaricaturistRequest>> Update(UpdateCaricaturistRequest request);
    Task<ServiceResult<CaricaturistDetailsResponse>> Details(int id);
    Task<BaseDataTableResponse<List<DetailsCaricaturistNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetCaricaturistFiltersRequest request);
    Task<ServiceResult> Delete(int id);
}