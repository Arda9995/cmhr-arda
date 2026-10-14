using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.Data.Models;

namespace Quark.API.Controllers;

[ApiController]
public class CategoryController(ICategoryService categoryService, INewsService newsService) : ApiPublicBaseController
{
	[HttpGet]
    public async Task<IActionResult> GetCategoryNewsList(string slug)
    {
        var newsCategoryType = await newsService.CheckCategoryTypeBySlug(slug);
        if (!newsCategoryType.IsSuccess || newsCategoryType.Data == 0)
        {
            return NotFound("Category not found");
        }

        var newsList = await newsService.GetNewsByCategorySlug(newsCategoryType.Data);
        if (!newsList.IsSuccess)
        {
            return BadRequest(newsList.Message);
        }

        return Ok(newsList);
    }

	[HttpGet]
	public async Task<IActionResult> GetMobileCategoryNewsList(string slug)
	{
		var newsCategoryType = await newsService.CheckCategoryTypeBySlug(slug);
		if (!newsCategoryType.IsSuccess || newsCategoryType.Data == 0)
		{
			return NotFound("Category not found");
		}

		var newsList = await newsService.GetMobileNewsByCategorySlug(newsCategoryType.Data);
		if (!newsList.IsSuccess)
		{
			return BadRequest(newsList.Message);
		}

		return Ok(newsList);
	}


	[HttpGet]
    public async Task<IActionResult> CategoryPageLoadMoreNews([FromQuery] int categoryId, [FromQuery] int page)
    {
        var pageSize = 10; 
        var newsResult = await newsService.GetMoreNewsByCategoryId(categoryId, page, pageSize);
        
        if (!newsResult.IsSuccess)
        {
            return BadRequest(new { success = false, message = newsResult.Message });
        }
        
        return Ok(newsResult);
    }
    
	[HttpGet]
	public async Task<IActionResult> DeleteCategoryNewsListInCache(int id)
	{
		var deleteCategoryNewsList = await categoryService.DeleteCategoryNewsListInCache(id);
		return Ok(deleteCategoryNewsList);
	}
}
