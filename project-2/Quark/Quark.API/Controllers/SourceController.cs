using AvvaMobile.Core.APIControllers;
using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.Data.Models;

namespace Quark.API.Controllers;

[ApiController]
public class SourceController(INewsService newsService) : ApiPublicBaseController
{
    [HttpGet]
    public async Task<IActionResult> NewsList(string slug)
    {
        var newsList = await newsService.GetNewsBySourceSlug(slug);
        if (!newsList.IsSuccess)
        {
            return BadRequest(newsList.Message);
        }
        return Ok(newsList);
    }

    [HttpGet]
    public async Task<IActionResult> LoadMoreNews([FromQuery] string slug, [FromQuery] int page)
    {
        var pageSize = 10;
        var newsResult = await newsService.GetMoreNewsBySourceSlug(slug, page, pageSize);

        if (!newsResult.IsSuccess)
        {
            return BadRequest(new { success = false, message = newsResult.Message });
        }
        
        var pageResponse = new
        {
            Items = newsResult.Data,
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