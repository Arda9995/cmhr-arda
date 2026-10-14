using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.NewsResponses;

namespace Quark.API.Controllers;

[ApiController]
public class AuthorController(IAuthorService authorService) : ApiPublicBaseController
{
    [HttpGet]
    public async Task<IActionResult> Authors()
    {
        var authors = await authorService.GetAuthorList();
        if (!authors.IsSuccess)
        {
            return BadRequest(authors.Message);
        }
        return Ok(authors);
    }

    [HttpGet]
    public async Task<IActionResult> AuthorDetails(string slug)
    {
        var author = await authorService.GetAuthorDetail(slug);
        if (!author.IsSuccess)
        {
            return BadRequest(author.Message);
        }
        return Ok(author);
    }

    [HttpGet]
    public async Task<IActionResult> GetAuthorTodayNewsList()
    {
        var newsList = await authorService.GetAuthorNewsTodayList();
        if (!newsList.IsSuccess)
        {
            return BadRequest(newsList.Message);
        }
        return Ok(newsList);
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredArticles([FromQuery] int authorId, [FromQuery] string dateRange, [FromQuery] int page = 1, [FromQuery] int pageSize = 40)
    {
        var result = await authorService.GetFilteredArticlesWithPagination(authorId, dateRange, page, pageSize);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }
        
        return Ok(result);
    }
}
