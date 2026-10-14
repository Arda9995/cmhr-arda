

using AvvaMobile.Core.Business;
using Quark.Business.Caching.Models.Requests.PageCachingRequests;
using Quark.Business.Caching.Models.Responses.PageCachingResponses;

namespace Quark.API.Business.Interfaces
{
	public interface IPageService
	{
		Task<ServiceResult<List<PageNewsCachingResponse>>> GetMainPageBlocks();
		Task<ServiceResult<PageCachingResponse>> GetByKey(PageCachingRequest request);
		Task<ServiceResult<PageCachingResponse>> GetBySlug(string slug);
	}
}
