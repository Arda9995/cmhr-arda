using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.NotificationRequests;

namespace Quark.CMS.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendWebPushNotification([FromBody]SendWebNotificationRequest request)
        {
            var result = await _notificationService.SendPushWebNotification(request);
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> SendMobilPushNotification([FromBody]SendMobilNotificationRequest request)
        {
            var result = await _notificationService.SendMobilPushNotification(request);
            return Ok(result);
        }
    }
}
