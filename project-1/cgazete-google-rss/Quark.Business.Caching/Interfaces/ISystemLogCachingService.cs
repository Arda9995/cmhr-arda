using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.SystemLogCachingResponses;

namespace Quark.Business.Caching.Interfaces;

public interface ISystemLogCachingService
{
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult<List<SelectListItem>>> GetLogTypesAsSelectListItems();
    Task<ServiceResult<List<SystemLogHomeCachingResponse>>> GetHomeLogs();
}