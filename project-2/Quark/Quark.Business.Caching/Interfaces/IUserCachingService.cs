using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.Business.Caching.Interfaces;

public interface IUserCachingService
{
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult<List<SelectListItem>>> Users();

    
}