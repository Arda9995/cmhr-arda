using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using AvvaMobile.Core;

namespace Quark.Web.Controllers
{
    public abstract class CumhuriyetBaseController : BaseController
    {
        protected IActionResult NotFoundView(string viewPath = "~/Views/Home/Page404.cshtml")
        {
            var statusCodePages = HttpContext.Features.Get<IStatusCodePagesFeature>();
            if (statusCodePages != null)
            {
                statusCodePages.Enabled = false;
            }
            HttpContext.Response.StatusCode = 404;
            return View(viewPath);
        }
    }
} 