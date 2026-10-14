using AvvaMobile.Core.Business;

namespace Quark.Notification.Adapter.Interfaces;

public interface IFCMService
    {
        /// <summary>
        /// Tek bir cihaza bildirim gönderir
        /// </summary>
        /// <param name="deviceToken">Cihaz token'ı</param>
        /// <param name="title">Bildirim başlığı</param>
        /// <param name="body">Bildirim içeriği</param>
        /// <param name="data">Ek veri (opsiyonel)</param>
        Task SendNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string> data = null);

        /// <summary>
        /// Birden fazla cihaza bildirim gönderir
        /// </summary>
        /// <param name="deviceTokens">Cihaz token'ları listesi</param>
        /// <param name="title">Bildirim başlığı</param>
        /// <param name="body">Bildirim içeriği</param>
        /// <param name="data">Ek veri (opsiyonel)</param>
        Task SendMulticastNotificationAsync(List<string> deviceTokens, string title, string body, Dictionary<string, string> data = null);

        /// <summary>
        /// Belirli bir konuya (topic) abone olan tüm cihazlara bildirim gönderir
        /// </summary>
        /// <param name="topic">Konu adı</param>
        /// <param name="title">Bildirim başlığı</param>
        /// <param name="body">Bildirim içeriği</param>
        /// <param name="data">Ek veri (opsiyonel)</param>
        Task<ServiceResult> SendTopicNotificationAsync(string topic, string title, string body, string imageUrl = null,Dictionary<string, string> data = null);

        /// <summary>
        /// Bir cihazı belirli bir konuya (topic) abone yapar
        /// </summary>
        /// <param name="deviceToken">Cihaz token'ı</param>
        /// <param name="topic">Konu adı</param>
        Task SubscribeToTopicAsync(string deviceToken, string topic);

        /// <summary>
        /// Bir cihazı belirli bir konudan (topic) çıkarır
        /// </summary>
        /// <param name="deviceToken">Cihaz token'ı</param>
        /// <param name="topic">Konu adı</param>
        Task UnsubscribeFromTopicAsync(string deviceToken, string topic);
    }