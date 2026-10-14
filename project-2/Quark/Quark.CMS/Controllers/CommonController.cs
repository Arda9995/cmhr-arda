using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;

namespace Quark.CMS.Controllers
{
    [Authorize]
    public class CommonController : ApiBaseController
    {
        private readonly ICommonService _commonService;

        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGenders()
        {
            var result = await _commonService.GetGenders();
            return Ok(result);
        }
    }
}