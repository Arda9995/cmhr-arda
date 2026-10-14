using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.CategoryCachingResponses;

namespace Quark.Business.Caching.Interfaces;

public interface ICategoryCachingService
{
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult> DeleteCategoryNewsList(int id);
    Task<ServiceResult<List<CategoryCachingResponse>>> GetAllCategories();
    Task<ServiceResult<CategoryCachingResponse>> GetCategoryById(int id);
    Task<ServiceResult<CategoryCachingResponse>> GetCategoryByName(string name);
    Task<ServiceResult<List<SelectListItem>>> GetCategoriesAsSelectListItems();
}