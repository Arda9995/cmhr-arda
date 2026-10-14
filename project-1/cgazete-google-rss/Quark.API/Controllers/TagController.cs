using AvvaMobile.Core.APIControllers;
using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;

namespace Quark.API.Controllers;

[ApiController]
public class TagController(INewsService newsService) : ApiPublicBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetTagNewsList(string slug)
    {
        var newsList = await newsService.GetNewsByTagSlug(slug);
        if (!newsList.IsSuccess)
        {
            return BadRequest(newsList.Message);
        }

        return Ok(newsList);
    }

    [HttpGet]
    public async Task<IActionResult> GetTagePageLoadMoreNews([FromQuery] string slug, [FromQuery] int page)
    {
        var pageSize = 10;
        var newsResult = await newsService.GetMoreNewsByTagSlug(slug, page, pageSize);

        if (!newsResult.IsSuccess)
        {
            return BadRequest(new { success = false, message = newsResult.Message });
        }

        var pageResponse = new
        {
            Items = newsResult.Data.Items,
            HasMoreItems = newsResult.Data.HasMoreItems
        };

        var response = new ServiceResult<object>()
        {
            IsSuccess = true,
            Data = pageResponse
        };

        return Ok(response);
    }
}