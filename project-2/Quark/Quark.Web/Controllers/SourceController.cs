using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Quark.API.Business.Interfaces;
using Quark.Data.Models;

namespace Quark.Web.Controllers
{
    public class SourceController : CumhuriyetBaseController
	{
        private readonly INewsService _newsService;
        public SourceController(INewsService newsService)
        {
            _newsService = newsService;
        }

         [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> NewsList(string slug)
        {
            var newsList = await _newsService.GetNewsBySourceSlug(slug);
            if (!newsList.IsSuccess || newsList.Data == null)
			{
				return NotFoundView();
			}
            return View("NewsList", newsList.Data);
        }

        [HttpGet]
        [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> LoadMoreNews(string slug, int page)
        {
            var pageSize = 10; 
            var newsResult = await _newsService.GetMoreNewsBySourceSlug(slug, page, pageSize);

            if (!newsResult.IsSuccess || newsResult.Data == null)
            {
                return Json(new { success = false, message = newsResult.Message });
            }

            return Json(new
            {
                success = true,
                items = newsResult.Data.Items,
                hasMoreItems = newsResult.Data.HasMoreItems
            });
        }
    }
}
