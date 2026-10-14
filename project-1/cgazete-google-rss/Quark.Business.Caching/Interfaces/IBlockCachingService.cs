using AvvaMobile.Core.Business;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.PageCachingResponses;

namespace Quark.Business.Caching.Interfaces;

public interface IBlockCachingService
{
    Task<ServiceResult> RefreshAll();
    Task<ServiceResult<List<SelectListItem>>> BlockTypes();
    Task<ServiceResult> RefreshBlockNews();
    Task<ServiceResult> RefreshBlockNewsWithKey(string key);
    Task<ServiceResult<List<PageNewsCachingResponse>>> GetBlockNews(List<string> blockKeys);
}