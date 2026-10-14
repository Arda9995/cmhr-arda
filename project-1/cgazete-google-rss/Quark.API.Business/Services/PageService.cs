using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Nest;
using Quark.API.Business.Interfaces;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Requests.PageCachingRequests;
using Quark.Business.Caching.Models.Responses.PageCachingResponses;
using Quark.Business.Caching.Services;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;

namespace Quark.API.Business.Services
{
    public class PageService : BaseService, IPageService
    {
        private readonly IPageCachingService _pageCachingService;
        private readonly INewsCachingService _newsCachingService;
        private readonly IBlockCachingService _blockCachingService;
        public PageService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, IPageCachingService pageCachingService, INewsCachingService newsCachingService, IBlockCachingService blockCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _pageCachingService = pageCachingService;
            _newsCachingService = newsCachingService;
            _blockCachingService = blockCachingService;
        }

        public async Task<ServiceResult<PageCachingResponse>> GetByKey(PageCachingRequest request)
        {
            var result = new ServiceResult<PageCachingResponse>();
            var page = await _pageCachingService.GetByKey(request);
            result.Data = page.Data;
            return result;
        }

        public async Task<ServiceResult<PageCachingResponse>> GetBySlug(string slug)
        {
            var result = new ServiceResult<PageCachingResponse>();
            var page = await _pageCachingService.GetBySlug(slug);
            result.Data = page.Data;
            return result;
        }

        public async Task<ServiceResult<List<PageNewsCachingResponse>>> GetMainPageBlocks()
        {
            var result = new ServiceResult<List<PageNewsCachingResponse>>();

            var blockList = new List<string>()
            {
                "AnaSayfa-SurdurulebilirliHaberleri",
                "AnaSayfa-DunyaHaberleri",
                "AnaSayfa-EkonomiHaberleri",
                "AnaSayfa-GundemHaberleri",
                "AnaSayfa-SliderAltiKutular",
                "AnaSayfa-MiniSlider",
                "AnaSayfa-AnaSlider",
                "AnaSayfa-SurmansetKutular",
                "AnaSayfa-SurmansetSlider",
                "Anasayfa-SporHaberleri",
                "Anasayfa-YasamHaberleriSlider",
                "Anasayfa-GurmeHaberleri",
                "Anasayfa-BilimTeknolojiHaberleri",
                "Anasayfa-KulturSanatHaberleri",
                "Anasayfa-KitapEkiHaberleri",
                "Anasayfa-OtomotivHaberleri",
                "Anasayfa-EnCokArananlarTab1",
                "Anasayfa-EnCokArananlarTab2",
                "Anasayfa-EnCokArananlarTab3",
                "Anasayfa-EnCokArananlarTab4",
                "AnaSayfa-AnketHaberi",
                "Anasayfa-YazarAlti",
                "Anasayfa-YasamHaberleri",
                "AnaSayfa-AnaSliderAltiKutular",
                "AnaSayfa-BlokDoldurucu",
                "Anasayfa-ModaHaberleri"
            };


            var cacheBlockNews = await _blockCachingService.GetBlockNews(blockList);

           

            result.Data = cacheBlockNews.Data;
            return result;
        }


    }
}
