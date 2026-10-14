using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.Data.Models;

namespace Quark.Web.Controllers;

public class CategoryController : CumhuriyetBaseController
{

	private readonly ICategoryService _categoryService;
	private readonly INewsService _newsService;

	public CategoryController(ICategoryService categoryService, INewsService newsService)
	{
		_newsService = newsService;
		_categoryService = categoryService;
	}

	[HttpPost]
	public async Task<IActionResult> GetGovernmentAnnouncementListByDate(DateTime date)
	{
		var newsResult = await _newsService.GetGovernmentAnnouncementListByDate(date);
		return Ok(newsResult);
	}

	[HttpGet]
	public async Task<IActionResult> LoadMoreNews(int categoryId, int page)
	{
		var pageSize = 10;
		var newsResult = await _newsService.GetMoreNewsByCategoryId(categoryId, page, pageSize);

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