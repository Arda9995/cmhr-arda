using AvvaMobile.Core;
using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.SurveyResponses;
using Quark.Business.Common.Caching;
using Quark.Data.Models;

namespace Quark.Web.Controllers
{
	public class NewsController : CumhuriyetBaseController
	{
		private readonly INewsService _newsService;
		private readonly IPageService _pageService;
		private readonly AppSettingsKeys _appSettingsKeys;
		public NewsController(INewsService newsService, IPageService pageService, AppSettingsKeys appSettingsKeys)
		{
			_appSettingsKeys = appSettingsKeys;
			_pageService = pageService;
			_newsService = newsService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[OutputCache(PolicyName = "DynamicTTL")]
		public async Task<IActionResult> Details(int id)
		{
			
			var newsDetail = await _newsService.GetNewsDetail(id);

			if (!newsDetail.IsSuccess || newsDetail == null)
			{
				return NotFoundView();
			}

			var isRedirect = await _newsService.CheckURLToRedirect(newsDetail.Data.NewsSlug, Request.Path);
			if (!isRedirect.IsSuccess)
			{
				return RedirectPermanent($"{_appSettingsKeys.Common_ClientWebSite}{isRedirect.Data.TrimStart('/')}");

            }

			 var newsType = newsDetail.Data.NewsTypeID;
			switch (newsType)
			{
				case (int)NewsTypesEnum.News:
					return PartialView("Details", newsDetail.Data);
				case (int)NewsTypesEnum.PhotoGallery:
					return PartialView("PhotoGalleryDetails", newsDetail.Data);
				case (int)NewsTypesEnum.VideoGallery:
					return PartialView("Details", newsDetail.Data);
				case (int)NewsTypesEnum.ColumnArticles:
					return PartialView("ColumnArticleDetails", newsDetail.Data);
				case (int)NewsTypesEnum.Ad:
					return PartialView("Details", newsDetail.Data);
				case (int)NewsTypesEnum.GovernmentAnnouncement:
					return PartialView("GovernmentAnnouncementDetails", newsDetail.Data);
				case (int)NewsTypesEnum.CaricaturistNews:
					return PartialView("Details", newsDetail.Data);
				case (int)NewsTypesEnum.ZodiacSigns:
					return PartialView("ZodiacSignDetails", newsDetail.Data);
				case (int)NewsTypesEnum.Survey:
					return PartialView("SurveyDetails", newsDetail.Data);
				default:
					return NotFoundView();
			}
		}
		public async Task<IActionResult> RedirectToFullSlug(int id)
		{
			var urlByID = await _newsService.GetUrlByID(id);

			if (urlByID == null || string.IsNullOrEmpty(urlByID.Data))
				return NotFound();

			var currentUrl = HttpContext.Request.Path.Value;

			if (string.Equals(currentUrl.TrimEnd('/'), urlByID.Data.TrimEnd('/'), StringComparison.OrdinalIgnoreCase))
			{
				return NotFoundView();
			}

			return RedirectPermanent(urlByID.Data);
		}

		[OutputCache(PolicyName = "DynamicTTL")]
		public async Task<IActionResult> DetailNextNews(int id)
		{
			var newsType = await _newsService.GetNewsType(id);
			switch (newsType.Data)
			{
				case (int)NewsTypesEnum.News:
					var newsDetail = await _newsService.GetNewsDetail(id);
					return PartialView("_DetailPartial", newsDetail.Data);
				case (int)NewsTypesEnum.PhotoGallery:
					var newsPhotoGalleryDetail = await _newsService.GetNewsDetail(id);
					return PartialView("_PhotoGalleryDetailsPartial", newsPhotoGalleryDetail.Data);
				case (int)NewsTypesEnum.VideoGallery:
					var newsVideoGalleryDetail = await _newsService.GetNewsDetail(id);
					return PartialView("_DetailPartial", newsVideoGalleryDetail.Data);
				case (int)NewsTypesEnum.ColumnArticles:
					var newsColumnArticlesDetail = await _newsService.GetNewsDetail(id);
					return PartialView("_ColumnArticleDetailsPartial", newsColumnArticlesDetail.Data);
				case (int)NewsTypesEnum.Ad:
					var newsAdDetail = await _newsService.GetNewsDetail(id);
					return PartialView("_DetailPartial", newsAdDetail.Data);
				case (int)NewsTypesEnum.GovernmentAnnouncement:
					var newsGovernmentAnnouncementDetail = await _newsService.GetNewsDetail(id);
					return PartialView("_GovernmentAnnouncementDetailsPartial", newsGovernmentAnnouncementDetail.Data);
				case (int)NewsTypesEnum.CaricaturistNews:
					var newsCaricaturistNewsDetail = await _newsService.GetNewsDetail(id);
					return PartialView("_DetailPartial", newsCaricaturistNewsDetail.Data);
				case (int)NewsTypesEnum.ZodiacSigns:
					var newsZodiacSignsDetail = await _newsService.GetNewsDetail(id);
					return PartialView("_ZodiacSignDetailsPartial", newsZodiacSignsDetail.Data);
				case (int)NewsTypesEnum.Survey:
					var newsSurveyDetail = await _newsService.GetNewsDetail(id);
					return PartialView("_SurveyDetailsPartial", newsSurveyDetail.Data);
				default:
					return NotFoundView();
			}
		}

		[OutputCache(PolicyName = "DynamicTTL")]
		public async Task<IActionResult> Search()
		{
			var pageNews = await _newsService.GetMostSearchedNewsBlock();
			return View(pageNews.Data);
		}

		[OutputCache(PolicyName = "DynamicTTL")]
		public async Task<IActionResult> SearchedNews(string search)
		{
			var newsList = await _newsService.GetNewsListBySearchedKey(search);
			return View(newsList.Data);
		}

		[HttpPost]
		public async Task<IActionResult> LiveSearch(string search)
		{
			var newsList = await _newsService.GetNewsListBySearchKey(search);
			return Ok(newsList.Data);
		}

		[HttpGet]
		[OutputCache(PolicyName = "DynamicTTL")]
		public async Task<IActionResult> LoadMoreSearchResults(string search, int page, string dateRange)
		{

			var newsResult = await _newsService.GetMoreNewsBySearchKey(search, page, dateRange);

			if (!newsResult.IsSuccess || newsResult.Data == null)
			{
				return Json(new { success = false, message = newsResult.Message });
			}

			return Json(new
			{
				success = true,
				items = newsResult.Data.Items,
				hasMoreItems = newsResult.Data.HasMoreItems,
				totalCount = newsResult.Data.TotalCount,
			});
		}

		[HttpPost]
		public async Task<IActionResult> VoteSurvey([FromBody] SurveyVoteModel model)
		{
			var result = await _newsService.VoteSurvey(model.SurveyId, model.OptionId);

			if (!result.IsSuccess)
			{
				return Json(new { success = false, message = result.Message });
			}

			return Json(new { success = true, message = result.Message });
		}

		[HttpGet]
		[OutputCache(PolicyName = "DynamicTTL")]
		public async Task<IActionResult> GetSurveyResult(int id)
		{
			var result = await _newsService.GetSurveyOptionResult(id);
			if (result.IsSuccess)
			{
				return Json(new { success = true, data = result.Data });
			}
			return Json(new { success = false, message = result.Message });
		}

		[HttpPost]
		public async Task<IActionResult> GetNewsByCategory(string slug)
		{
			var categoryID = await _newsService.CheckCategoryTypeBySlug(slug);
			if (categoryID.Data != 0)
			{
				var response = await _newsService.GetNewsByCategorySlug(categoryID.Data);
				return Ok(response.Data);
			}
			return Json(new { success = false, message = "Category not found." });
		}

		[HttpGet]
		public async Task<IActionResult> AddViewCount(int id)
		{
			var result = await _newsService.AddViewCount(id);
			if (result.IsSuccess)
			{
				return Json(new { success = true, message = "Success" });
			}
			return Json(new { success = false, message = "Error" });
		}
	}

}
