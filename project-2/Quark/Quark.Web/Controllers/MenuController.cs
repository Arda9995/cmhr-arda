using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Quark.API.Business.Interfaces;

namespace Quark.Web.Controllers
{
    public class MenuController : CumhuriyetBaseController
	{
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
        [HttpGet]
        [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> ListByKey(string key)
        {
            var result = await _menuService.ListMenusByKeyAsync(key);
            return Ok(result);
        }       

        [HttpGet]
        [OutputCache(PolicyName = "DynamicTTL")]
        public async Task<IActionResult> List()
        {
            var result = await _menuService.ListMenusAsync();
            return Ok(result);
        }
    }
}
