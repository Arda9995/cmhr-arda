using AvvaMobile.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.PageResponses;
using Quark.Web.Models;
using System.Diagnostics;

namespace Quark.Web.Controllers
{
    public class HomeController : CumhuriyetBaseController
	{
        private readonly ILogger<HomeController> _logger;
        private readonly IPageService _pageService;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly INewsService _newsService;


        public HomeController(ILogger<HomeController> logger, IPageService pageService, IAuthorService authorService, IBookService bookService, INewsService newsService)
        {
            _logger = logger;
            _pageService = pageService;
            _authorService = authorService;
            _bookService = bookService;
            _newsService = newsService;
        }

        [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> Index()
        {
            var pageNews = await _pageService.GetMainPageBlocks();
            var books = await _bookService.GetAllBooks();
            var authors = await _authorService.GetHomePageAuthorList();
            var zodiacSignNews = await _newsService.GetZodiacSignNewsListForIndex();
            var governmentAnnouncements = await _newsService.GetDailyGovernmentAnnouncement();

            var zodiacSignListItem = zodiacSignNews.Data.ZodiacSignItems
                .Select(x => new ZodiacSignBlock
                {
                    NewsID = x.NewsID,
                    Slug = x.Slug,
                    Icon = x.Icon,
                    PublishDate = x.PublishDate,
                    ZodiacSignType = x.ZodiacSignType
                })
                .ToList();

            var response = new MainPageResponse
            {
                GovernmentAnnouncementItems = governmentAnnouncements.Data,
                AuthorCachingResponse = authors.Data,
                BookCachingResponses = books.Data,
                PageNewsCachingResponses = pageNews.Data,
                AstrologerImageURL = zodiacSignNews.Data.AstrologerImageURL,
                AstrologerName = zodiacSignNews.Data.AstrologerName,
                ZodiacSignItems = zodiacSignListItem,
                AstrologieDateStr = zodiacSignNews.Data.DateStringForZodiacSignDateRange
            };

            var userAgent = Request.Headers["User-Agent"].ToString().ToLower();

            if (userAgent.Contains("android") || userAgent.Contains("iphone") || userAgent.Contains("mobile"))
            {
                return View("IndexMobile",response);
            }

            return View("Index", response);
        }


        [OutputCache(PolicyName = "DynamicTTL")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [OutputCache(PolicyName = "DynamicTTL")]
        [Route("Home/404")]
		public IActionResult PageNotFound()
		{
			Response.StatusCode = 404;
			return View("Page404");
		}

        [OutputCache(PolicyName = "DynamicTTL")]
        [Route("Home/500")]
		public IActionResult PageSystemError()
		{
			Response.StatusCode = 500;
			return View("Page404");
		}
	}
}
