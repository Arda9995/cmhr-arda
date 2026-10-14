using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.NotificationRequests;
using Quark.Data;
using Quark.Notification.Adapter;
using Quark.Notification.Adapter.Interfaces;
using Quark.Notification.Adapter.Models.Requests;
using Quark.Resources;

namespace Quark.CMS.Business.Services
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly NetmeraAdapter _netmeraAdapter;
        private readonly IFCMService _fcmService;
        private readonly string GLOBAL_TOPIC = "all";
        private readonly string Click_Action = "click_action";
        public NotificationService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, NetmeraAdapter netmeraAdapter, IFCMService fcmService) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _netmeraAdapter = netmeraAdapter;
            _fcmService = fcmService;
        }

        public async Task<ServiceResult> SendPushWebNotification(SendWebNotificationRequest request)
        {
            var result = new ServiceResult();

            if (string.IsNullOrEmpty(request.Title))
            {
                result.SetError("Bildirim başlığı boş olamaz");
                return result;
            }

            if (string.IsNullOrEmpty(request.Message))
            {
                result.SetError("Bildirim mesajı boş olamaz");
                return result;
            }

            var netmeraRequest = new NetmeraNotificationRequest
            {
                Message = new NetmeraMessage
                {
                    Title = request.Title,
                    Text = request.Message,
                    IconUrl = "https://www.cumhuriyet.com.tr/Archive/yazarlar/foto/cumhuriyet.png",
                    WpChromeImage = request.WpChromeImage,
                    InteractionRequired = true,
                    Platforms = new List<string> { "CHROME", "MACOS", "FIREFOX", "OPERA", "EDGE", "YANDEX" },
                    Click = new NetmeraClick
                    {
                        Url = request.Url
                    }
                },
                Target = new NetmeraTarget
                {
                    SendToAll = true,
                },
            };

            var response = await _netmeraAdapter.SendNotificationAsync(netmeraRequest);

            if (!result.IsSuccess)
            {
                result.SetError($"Bildirim gönderilirken hata oluştu: {string.Join(", ", result.Message)}");
                return result;
            }

            return response;
        }

        public async Task<ServiceResult> SendMobilPushNotification(SendMobilNotificationRequest request)
        {
            var result = new ServiceResult();
            
            if (string.IsNullOrEmpty(request.Title))
            {
                result.SetError("Bildirim başlığı boş olamaz");
                return result;
            }
            
            var dataBody = new Dictionary<string, string> 
            { 
            	{ "IdHaber", request.NewsID.ToString() }, 
            	{ "IdHaberTipi", request.NewsTypeName }, 
            	{ "click_action", Click_Action } 
            }; 
            
            var fcmSendNotification = await _fcmService.SendTopicNotificationAsync(GLOBAL_TOPIC, 
            	request.Title,  
            	request.Description, 
            	imageUrl:request.ImageURL,
                data:dataBody);
            
            if (!fcmSendNotification.IsSuccess) 
            { 
            	result.SetError(_localizer["Error"]); 
            	return result; 
            } 
            
            return result;
        }
    }
}
