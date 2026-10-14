using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.CategoryResponses;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;

namespace Quark.API.Business.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryCachingService _categoryCachingService;
        public CategoryService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, ICategoryCachingService categoryCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _categoryCachingService = categoryCachingService;
        }

        public async Task<ServiceResult<List<GetCategoryResponse>>> GetAll()
        {
            var result = new ServiceResult<List<GetCategoryResponse>>();

            var category = await _categoryCachingService.GetAllCategories();

            if (category != null)
            {
               var categoryData = category.Data.Select(x => new GetCategoryResponse
                {
                    Breadcrumb = x.Breadcrumb,
                    ID = x.ID,
                    IsEnabled = x.IsEnabled,
                    Map = x.Map,
                    Name = x.Name,
                    ParentID = x.ParentID,
                    PhotoUrl = x.PhotoUrl,
                    SeoDescription = x.SeoDescription,
                    SeoTitle = x.SeoTitle,
                    Url = x.Url,
                }).ToList();

                result.Data = categoryData;
            }
            return result;
        }

        public async Task<ServiceResult<GetCategoryResponse>> GetById(int id)
        {
            var result = new ServiceResult<GetCategoryResponse>();

            var category = await _categoryCachingService.GetCategoryById(id);

            if (category != null)
            {
                var categoryResult = new GetCategoryResponse
                {
                    ID = category.Data.ID,
                    Name = category.Data.Name,
                    Breadcrumb = category.Data.Breadcrumb,
                    IsEnabled = category.Data.IsEnabled,
                    Map = category.Data.Map,
                    ParentID = category.Data.ParentID,
                    PhotoUrl = category.Data.PhotoUrl,
                    SeoDescription = category.Data.SeoDescription,
                    SeoTitle = category.Data.SeoTitle,
                    Url = category.Data.Url
                };

                result.Data = categoryResult;
            }
            return result;
        }

        public async Task<ServiceResult<GetCategoryResponse>> GetByName(string name)
        {
            var result = new ServiceResult<GetCategoryResponse>();

            var category = await _categoryCachingService.GetCategoryByName(name);

            if (category != null)
            {
                var categoryResult = new GetCategoryResponse
                {
                    ID = category.Data.ID,
                    Name = category.Data.Name,
                    Breadcrumb = category.Data.Breadcrumb,
                    IsEnabled = category.Data.IsEnabled,
                    Map = category.Data.Map,
                    ParentID = category.Data.ParentID,
                    PhotoUrl = category.Data.PhotoUrl,
                    SeoDescription = category.Data.SeoDescription,
                    SeoTitle = category.Data.SeoTitle,
                    Url = category.Data.Url
                };

                result.Data = categoryResult;
            }
            return result;
        }

        public async Task<ServiceResult> DeleteCategoryNewsListInCache(int id)
        {
            var delete = await _categoryCachingService.DeleteCategoryNewsList(id);
            return delete;
        }
    }
}
