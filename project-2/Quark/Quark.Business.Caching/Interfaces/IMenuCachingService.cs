using AvvaMobile.Core.Business;
using Quark.Business.Caching.Models.Requests.MenuCachingRequests;
using Quark.Business.Caching.Models.Responses.MenuCachingResponses;

namespace Quark.Business.Caching.Interfaces;

public interface IMenuCachingService
{
    Task<ServiceResult<MenuCachingResponse>> GetByID(int id);
    Task<ServiceResult<List<MenuCachingResponse>>> GetAll();
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult> RefreshSingle(int id);
    Task<ServiceResult> RemoveSingle(int id);
}