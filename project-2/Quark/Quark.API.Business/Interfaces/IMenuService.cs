using AvvaMobile.Core.Business;
using Quark.API.Business.Models.Responses.MenuResponses;
using Quark.Business.Caching.Models.Responses.MenuCachingResponses;

namespace Quark.API.Business.Interfaces;

public interface IMenuService
{
    Task<ServiceResult<ListMenusResponse>> ListMenusByIDAsync(int id);
    Task<ServiceResult<List<MenuCachingResponse>>> ListMenusAsync();
    Task<ServiceResult<List<MenuCachingResponse>>> ListMenusByKeyAsync(string key);

}