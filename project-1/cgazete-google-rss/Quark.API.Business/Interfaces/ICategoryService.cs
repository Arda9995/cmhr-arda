using AvvaMobile.Core.Business;
using Quark.API.Business.Models.Responses.CategoryResponses;

namespace Quark.API.Business.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResult<List<GetCategoryResponse>>> GetAll();
        Task<ServiceResult<GetCategoryResponse>> GetById(int id);
        Task<ServiceResult<GetCategoryResponse>> GetByName(string name);
        Task<ServiceResult> DeleteCategoryNewsListInCache(int id);
    }
}
