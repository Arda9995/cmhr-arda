using AvvaMobile.Core.Business;
using Quark.API.Business.Models.Responses.LocationResponses;

namespace Quark.API.Business.Interfaces
{
    public interface ILocationService
    {
        Task<ServiceResult<List<LocationResponse>>> GetCitiesByCountryAsync(int countryId);
        Task<ServiceResult<List<LocationResponse>>> GetDistrictsByCityAsync(int cityId);
    }
} 