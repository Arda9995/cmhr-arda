using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.PromptCachingResponses;

namespace Quark.Business.Caching.Interfaces;

public interface IPromptCachingService
{
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult<List<SelectListItem>>> GetPromptTypesAsSelectListItems();
    Task<ServiceResult<List<PromptCachingResponse>>> GetPrompts();
    Task<ServiceResult<PromptCachingResponse>> GetPromptByID(int id);

}