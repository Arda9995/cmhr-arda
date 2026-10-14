using AvvaMobile.Core.Business;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.SurveyResponses;

namespace Quark.API.Business.Services
{
    public class SurveyService : ISurveyService
    {
        public async Task<ServiceResult<GetSurveyListResponse>> GetAllSurveys()
        {
            var result = new ServiceResult<GetSurveyListResponse>();
            return result;
        }

        public async Task<ServiceResult<GetSurveyDetailResponse>> GetSurveyDetails()
        {
            var result = new ServiceResult<GetSurveyDetailResponse>();
            return result;
        }
    }
}
