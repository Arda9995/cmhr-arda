using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.PageResponses;
using Quark.Business.Caching.Models.Requests.PageCachingRequests;

namespace Quark.API.Controllers;

[Route("Content/Pages")]
[ApiController]
public class PageController(IPageService pageService) : ApiBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetPageBlocks([FromHeader] PageCachingRequest request)
    {
        var result = await pageService.GetByKey(request);
        var data = new PageGetResponse
        {
            Content = result.Data.Content
        };
        return Ok(data);
    }
}