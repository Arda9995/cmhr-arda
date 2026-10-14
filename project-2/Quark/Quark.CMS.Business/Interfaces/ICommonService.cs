using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.CMS.Business.Models.Responses.CommonResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface ICommonService
    {
        ServiceResult<List<SelectListItem>> GetDefaultSelectItems();

        Task<ServiceResult<List<SelectListItem>>> GetGenders();

        Task<ServiceResult<List<SelectListItem>>> GetCountries();

        Task<ServiceResult<List<SelectListItem>>> GetCitiesOfTurkey();

        Task<ServiceResult<List<SelectListItem>>> GetCitiesByCountryId(int id);

        Task<ServiceResult<List<SelectListItem>>> GetDistrictsByCityID(int id);

        Task<ServiceResult<List<SelectListItem>>> GetNeighbourByDistrictID(int id);

    }
}