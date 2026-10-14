using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.TagCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;

namespace Quark.API.Business.Services
{
    public class TagService : BaseService, ITagService
    {
        private readonly ITagCachingService _tagCachingService;
        public TagService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, ITagCachingService tagCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _tagCachingService = tagCachingService;
        }

        public async Task<ServiceResult<TagCachingResponse>> GetByText(string text)
        {
            var result = new ServiceResult<TagCachingResponse>();
            var tag = await _tagCachingService.GetByText(text);
            result.Data = tag.Data;
            return result;
        }

        public async Task<ServiceResult<List<TagCachingResponse>>> List()
        {
            var result = new ServiceResult<List<TagCachingResponse>>();
            var tags = await _tagCachingService.Get();
            result.Data = tags.Data;
            return result;
        }
    }
}
