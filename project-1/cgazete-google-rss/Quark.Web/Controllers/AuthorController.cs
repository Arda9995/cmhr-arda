using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.NewsResponses;

namespace Quark.Web.Controllers
{
	public class AuthorController : CumhuriyetBaseController
	{
		private readonly IAuthorService _authorService;
		public AuthorController(IAuthorService authorService)
		{
			_authorService = authorService;
		}

         [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> Authors()
		{
			var authors = await _authorService.GetAuthorList();
			if (!authors.IsSuccess)
			{
				return NotFoundView();
			}
			return View(authors.Data);
		}

         [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> AuthorDetails(string slug)
		{
			var author = await _authorService.GetAuthorDetail(slug);
			if (!author.IsSuccess)
			{
				return NotFoundView();
			}
			return View(author.Data);
		}

        [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> AuthorNewsList()
		{
			var newsList = await _authorService.GetAuthorNewsTodayList();
			if (!newsList.IsSuccess)
			{
				return NotFoundView();
			}
			return View(newsList.Data);
		}

         [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> AuthorCategoryNewsList(string slug)
        {
            var newsList = await _authorService.GetAuthorCategoryNewsTodayList(slug);
            if (!newsList.IsSuccess)
            {
				return NotFoundView();
			}
            return View(newsList.Data);
        }

        [HttpGet]
		public async Task<IActionResult> GetFilteredArticles(int authorId, string dateRange, int page = 1, int pageSize = 10)
		{
			var result = await _authorService.GetFilteredArticlesWithPagination(authorId, dateRange, page, pageSize);
			
			if (!result.IsSuccess)
			{
				return BadRequest(result.Message);
			}
			
			return Json(result.Data);
		}
	}
}
