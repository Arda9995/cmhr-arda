using AvvaMobile.Core.APIControllers;
using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.PageResponses;

namespace Quark.API.Controllers;

[ApiController]
public class HomeController(
    ILogger<HomeController> logger,
    IPageService pageService,
    IAuthorService authorService,
    IBookService bookService)
    : ApiPublicBaseController
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly string mainPageKey = "AnaSayfa";

    [HttpGet]
    public async Task<IActionResult> GetMainPage()
    {
        var pageNews = await pageService.GetMainPageBlocks();
        var books = await bookService.GetAllBooks();
        var authors = await authorService.GetHomePageAuthorList();

        if (!pageNews.IsSuccess || !books.IsSuccess || !authors.IsSuccess)
        {
            return BadRequest(new
            {
                PageNewsError = pageNews.Message,
                BooksError = books.Message,
                AuthorsError = authors.Message
            });
        }
        
        var pageResponse = new
        {
            PageNews = pageNews.Data,
            Books = books.Data,
            Authors = authors.Data
        };
        
        var response = new ServiceResult<object>()
        {
            IsSuccess = true,
            Data = pageResponse
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> IndexMobile()
    {
        var pageNews = await pageService.GetMainPageBlocks();
        var books = await bookService.GetAllBooks();
        var authors = await authorService.GetHomePageAuthorList();

        if (!pageNews.IsSuccess || !books.IsSuccess || !authors.IsSuccess)
        {
            return BadRequest(new
            {
                PageNewsError = pageNews.Message,
                BooksError = books.Message,
                AuthorsError = authors.Message
            });
        }

        var mainPageResponse = new MainPageResponse
        {
            AuthorCachingResponse = authors.Data,
            BookCachingResponses = books.Data,
            PageNewsCachingResponses = pageNews.Data
        };

        var response = new ServiceResult<MainPageResponse>()
        {
            IsSuccess = true,
            Data = mainPageResponse
        };

        return Ok(response);
    }

    [HttpGet]
    public IActionResult Error404()
    {
        return NotFound();
    }
} 