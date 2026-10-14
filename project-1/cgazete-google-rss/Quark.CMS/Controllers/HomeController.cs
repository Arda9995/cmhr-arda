using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using GoogleTrendsApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.Notification.Adapter;

namespace Quark.CMS.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IHomeService _service;
        private readonly IGoogleTrendsService _googleTrendsService;
        private readonly NetmeraAdapter _netmeraAdapter;

        public HomeController(IHomeService service, IGoogleTrendsService googleTrendsService, NetmeraAdapter netmeraAdapter)
        {
            _service = service;
            _googleTrendsService = googleTrendsService;
            _netmeraAdapter = netmeraAdapter;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _service.Index();
            return View(result.Data);
        }
        
        public IActionResult Error404()
        {
            return View();
        }
        
        public IActionResult Error500()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> GetAllNewsPaged()
        {
            var result = await _service.GetNewsPaged(new BaseDataTableRequest(Request));

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrends(Category category)
        {
            var result = await _googleTrendsService.GetRealtimeSearches(category);

            return PartialView("_GoogleTrends", result.Data);
        }
        
        public IActionResult GetCategories(string searchTerm)
        {
            var result = _googleTrendsService.GetCategories();

            return Json(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyNewsInformation()
        {
            var result = await _service.GetDailyNewsInformation();
            return Json(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetTodayNewsDetails()
        {
            var result = await _service.GetTodayNewsDetails();
            return Json(result.Data);
        }     
        
        [HttpGet]
        public async Task<IActionResult> GetMostReadedNewsDetails()
        {
            var result = await _service.GetMostReadedNewsDetails();
            return Json(result.Data);
        }      
        
        [HttpGet]
        public async Task<IActionResult> GetUnpublishedNewsCount()
        {
            var result = await _service.GetUnpublishedNewsCount();
            return Json(result.Data);
        }    
        
        [HttpGet]
        public async Task<IActionResult> GetNewsTypeDetails()
        {
            var result = await _service.GetNewsTypeDetails();
            return Json(result.Data);
        } 
        
        [HttpGet]
        public async Task<IActionResult> GetLastPublishedNews()
        {
            var result = await _service.GetLastPublishedNews();
            return Json(result.Data);
        }  

        [HttpGet]
        public async Task<IActionResult> GetNewsChartDataByCategories()
        {
            var result = await _service.GetNewsChartDataByCategories();
            return Json(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsChartDataByDailyNews()
        {
            var result = await _service.GetNewsChartDataByDailyNews();
            return Json(result.Data);
        }

    }
}