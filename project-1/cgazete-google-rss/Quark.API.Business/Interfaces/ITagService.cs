using AvvaMobile.Core.Business;
using Quark.Business.Caching.Models.Responses.TagCachingResponses;

namespace Quark.API.Business.Interfaces
{
    public interface ITagService
    {
        Task<ServiceResult<List<TagCachingResponse>>> List();
        Task<ServiceResult<TagCachingResponse>> GetByText(string text);
    }
}
