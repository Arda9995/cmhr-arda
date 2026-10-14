using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.NewsIntegrationRequest;
using Quark.CMS.Business.Models.Responses.NewsIntegrationResponses;

namespace Quark.CMS.Business.Interfaces;

public interface INewsIntegrationService
{
    Task<BaseDataTableResponse<List<ListNewsIntegrationItemResponse>>> GetAllPagedAsync(BaseDataTableRequest tableRequest);
    Task<ServiceResult<CreateNewsIntegrationRequest>> CreateAsync(long? newsId);

    Task<ServiceResult<CreateNewsIntegrationRequest>> CreateAsync(CreateNewsIntegrationRequest request);

    Task<ServiceResult<UpdateNewsIntegrationRequest>> UpdateAsync(long newsIntegrationId);

    Task<ServiceResult<UpdateNewsIntegrationRequest>> UpdateAsync(UpdateNewsIntegrationRequest request);
}