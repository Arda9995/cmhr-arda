using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Quark.API.Business.Interfaces;
using Quark.Data.Models;
using Quark.Data.Models.Common;

namespace Quark.Web.Controllers
{
	public class SlugController : CumhuriyetBaseController
	{
		private readonly ISlugService _slugService;
		private readonly IPageService _pageService;
		private readonly INewsService _newsService;
		public SlugController(ISlugService slugService, INewsService newsService, IPageService pageService)
		{
			_newsService = newsService;
			_slugService = slugService;
			_pageService = pageService;
		}

         [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> SlugHandler(string slug)
		{
			var slugInfo = await _slugService.GetSlugInfoAsync(slug);

			if (slugInfo.Data == EntityTypeEnum.Categories)
			{
				var newsCategoryType = await _newsService.CheckCategoryTypeBySlug(slug);
				if (!newsCategoryType.IsSuccess)
				{
                    return NotFoundView();
				}
				switch (newsCategoryType.Data)
				{
					case (int)CategoryEnum.GovernmentAnnouncement:
						var newsGovernmentAnnouncementList = await _newsService.GetNewsByCategorySlug(newsCategoryType.Data);
						return View("~/Views/Category/GovernmentAnnouncementList.cshtml", newsGovernmentAnnouncementList.Data);
					case (int)CategoryEnum.Astrologie:
						var newsZodiacSignsList = await _newsService.GetNewsByCategorySlug(newsCategoryType.Data);
						return View("~/Views/Category/ZodiacSignList.cshtml", newsZodiacSignsList.Data);
					case (int)CategoryEnum.Survey:
						var newsSurveyList = await _newsService.GetNewsByCategorySlug(newsCategoryType.Data);
						return View("~/Views/Category/SurveyList.cshtml", newsSurveyList.Data);
					default:
						var newsList = await _newsService.GetNewsByCategorySlug(newsCategoryType.Data);
						return View("~/Views/Category/NewsList.cshtml", newsList.Data);
				}
			}

			if (slugInfo.Data == EntityTypeEnum.Pages)
			{
				var result = await _pageService.GetBySlug(slug);
				return View("~/Views/Pages/Details.cshtml", result.Data);
			}

            return NotFoundView();
		}
	}
}
