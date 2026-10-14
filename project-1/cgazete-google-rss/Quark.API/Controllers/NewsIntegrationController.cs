using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;

namespace Quark.API.Controllers;

//[Authorize(AuthenticationSchemes = "BasicAuthentication")]
public class NewsIntegrationController(INewsIntegrationService newsIntegrationService) : ApiPublicBaseController
{
    [HttpGet]
    public async Task<IActionResult> PageCategories()
    {
        var response = await newsIntegrationService.GetPageCategoriesAsync();
        return Ok(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> Stories(long? id)
    {
        var response = await newsIntegrationService.GetListStoriesAsync(id);   
        return Ok(response);
    }
    
    [HttpGet]
    public async Task<IActionResult> Story(long id)
    {
        var response = await newsIntegrationService.GetStoryAsync(id);
        return Ok(response);
    }
}