using AvvaMobile.Core.Business;
using Quark.Business.Caching.Models.Responses.TagCachingResponses;
using Quark.Data.Models;

namespace Quark.Business.Caching.Interfaces
{
    public interface ITagCachingService
    {
        Task<ServiceResult> RefreshAll();
        Task<ServiceResult<List<TagCachingResponse>>> Get();
        Task<ServiceResult<TagCachingResponse>> GetByText(string text);
        Task<ServiceResult> RefreshAllTagInElastic();
        Task<ServiceResult<List<Tag>>> GetTagsElastic(List<int> ids);
        Task<ServiceResult<bool>> UpdateTagElastic(Tag tag);
        Task<ServiceResult<bool>> InsertTagElastic(Tag tag);
    }
}