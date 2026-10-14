using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.SurveyRequests;
using Quark.CMS.Business.Models.Responses.SurveyResponses;

namespace Quark.CMS.Business.Interfaces;

public interface ISurveyService
{
    ServiceResult<GetSurveyFiltersRequest> GetAllPagedFilters();
    Task<BaseDataTableResponse<List<ListResponseSurveyItem>>> GetAllPaged(BaseDataTableRequest tableRequest, GetSurveyFiltersRequest request);
    ServiceResult<CreateSurveyRequest> Create();
    Task<ServiceResult<CreateSurveyRequest>> Create(CreateSurveyRequest request);
    Task<ServiceResult<UpdateSurveyRequest>> Update(int id);
    Task<ServiceResult<UpdateSurveyRequest>> Update(UpdateSurveyRequest request);
    Task<ServiceResult<SurveyDetailsResponse>> Details(int id);
    Task<ServiceResult> Delete(int id);
    Task<ServiceResult> EnableSurvey(int id);
    Task<ServiceResult> RefreshAnswers(int id);
}