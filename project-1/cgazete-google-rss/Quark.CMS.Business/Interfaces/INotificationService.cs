using AvvaMobile.Core.Business;
using Quark.CMS.Business.Models.Requests.NotificationRequests;

namespace Quark.CMS.Business.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResult> SendPushWebNotification(SendWebNotificationRequest request);
        Task<ServiceResult> SendMobilPushNotification(SendMobilNotificationRequest request);
    }
}
