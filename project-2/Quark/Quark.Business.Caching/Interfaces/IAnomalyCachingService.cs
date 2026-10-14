using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.Business.Caching.Interfaces;

public interface IAnomalyCachingService
{
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult<List<SelectListItem>>> AnomalyDataTypesAsSelectListItems();
    Task<ServiceResult<List<SelectListItem>>> AnomalyIssueTypesAsSelectListItems();
}