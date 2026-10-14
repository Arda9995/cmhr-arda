using AvvaMobile.Core.Business;
using Quark.API.Business.Models.Responses.SurveyResponses;

namespace Quark.API.Business.Interfaces
{
    public interface ISurveyService
    {
        Task<ServiceResult<GetSurveyListResponse>> GetAllSurveys();
        Task<ServiceResult<GetSurveyDetailResponse>> GetSurveyDetails();
    }
}
