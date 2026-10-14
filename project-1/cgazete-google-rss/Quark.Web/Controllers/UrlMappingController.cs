using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Quark.Data;
using Quark.Web.Middlewares.Interfaces;

namespace Quark.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlMappingController : ControllerBase
	{
		private readonly IUrlMappingService _urlMappingService;
		private readonly QuarkDataContext _context;
        private readonly IEndpointRouteBuilder _endpoints;
        private readonly ILogger<UrlMappingController> _logger;

		public UrlMappingController(
            IUrlMappingService urlMappingService, 
            QuarkDataContext context,
            IEndpointRouteBuilder endpoints,
            ILogger<UrlMappingController> logger)
		{
			_urlMappingService = urlMappingService;
			_context = context;
            _endpoints = endpoints;
            _logger = logger;
		}

		[HttpPost("refresh-cache")]
		public IActionResult RefreshUrlMappingsCache()
		{
            try
            {
                _urlMappingService.RefreshUrlMappingsCache();
                return Ok(new { success = true, message = "URL mappings cache refreshed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh URL mappings cache");
                return StatusCode(500, new { success = false, message = "Error refreshing URL mappings cache" });
            }
		}
	}
}