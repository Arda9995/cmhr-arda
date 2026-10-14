using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Data.Models.Common;
using Quark.Resources;

namespace Quark.API.Business.Services
{
    public class SlugService : BaseService, ISlugService
    {
        private readonly IPageCachingService _pageCachingService;
        private readonly ICategoryCachingService _categoryCachingService;
        public SlugService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, IPageCachingService pageCachingService, ICategoryCachingService categoryCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _pageCachingService = pageCachingService;
            _categoryCachingService = categoryCachingService;
        }

        public async Task<ServiceResult<EntityTypeEnum>> GetSlugInfoAsync(string slug)
        {
            var result = new ServiceResult<EntityTypeEnum>();
             
            var pages = await _pageCachingService.GetWebPages();
            var categories = await _categoryCachingService.GetAllCategories();

            if (pages.Data != null && pages.Data.Any(x => x.Slug == slug))
            {
                result.Data = EntityTypeEnum.Pages;
                return result;
            }


            if (categories.Data != null && categories.Data.Any(x => x.Url == slug))
            {
                result.Data = EntityTypeEnum.Categories;
                return result;
            }

            return result;
        }
    }
}
