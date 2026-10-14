using Microsoft.AspNetCore.Mvc;
using Quark.Hangfire.Business.Interfaces;
using System.Threading.Tasks;

namespace Quark.Hangfire.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexController : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAANews([FromServices] IAgencyHangfireService service)
        {
           var result = await service.GetAANews();

            return Ok(result);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetRssItems([FromServices] IAgencyHangfireService service)
        {
            var result = await service.GetRssItemsAsync();

            return Ok(result);
        }
    }
}
