using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;
using Exception = System.Exception;
using IFCMService = Quark.Notification.Adapter.Interfaces.IFCMService;

namespace Quark.Notification.Adapter;

public class FCMAdapter : BaseService, IFCMService
{
    private readonly FirebaseMessaging _messaging;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    
    public FCMAdapter(IHttpContextAccessor httpContext, QuarkDataContext context,
        IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IConfiguration configuration, IWebHostEnvironment env) : base(
        httpContext, context, localizer, appSettingsKeys, cache)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _env = env ?? throw new ArgumentNullException(nameof(env));
        try
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                var firebaseConfig = _configuration.GetSection("Firebase").Get<FirebaseConfig>();
                if (string.IsNullOrEmpty(firebaseConfig?.CredentialsPath))
                {
                    throw new InvalidOperationException("Firebase credentials path is not configured.");
                }
                
                var credentialsPath = Path.Combine(_env.WebRootPath, firebaseConfig.CredentialsPath.TrimStart('/'));

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(credentialsPath)
                });
            }

            _messaging = FirebaseMessaging.DefaultInstance;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    public async Task SendNotificationAsync(string deviceToken, string title, string body,
        Dictionary<string, string> data = null)
    {
        if (string.IsNullOrEmpty(deviceToken))
            throw new ArgumentException("Device token cannot be null or empty", nameof(deviceToken));
        if (string.IsNullOrEmpty(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));
        if (string.IsNullOrEmpty(body))
            throw new ArgumentException("Body cannot be null or empty", nameof(body));

        try
        {
            var message = new Message()
            {
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data,
                Token = deviceToken
            };

            var response = await _messaging.SendAsync(message);
        }
        catch (FirebaseException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task SendMulticastNotificationAsync(List<string> deviceTokens, string title, string body,
        Dictionary<string, string> data = null)
    {
        if (deviceTokens == null || deviceTokens.Count == 0)
            throw new ArgumentException("Device tokens list cannot be null or empty", nameof(deviceTokens));
        if (string.IsNullOrEmpty(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));
        if (string.IsNullOrEmpty(body))
            throw new ArgumentException("Body cannot be null or empty", nameof(body));

        try
        {
            var message = new MulticastMessage()
            {
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = data,
                Tokens = deviceTokens
            };

            var response = await _messaging.SendMulticastAsync(message);

            if (response.FailureCount > 0)
            {
                for (int i = 0; i < response.Responses.Count; i++)
                {
                    if (!response.Responses[i].IsSuccess)
                    {
                        
                    }
                }
            }
        }
        catch (FirebaseException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<ServiceResult> SendTopicNotificationAsync(string topic, string title, string body, string imageUrl = null,
        Dictionary<string, string> data = null)
    {
        var result = new ServiceResult();
        
        if (string.IsNullOrEmpty(topic))
            result.SetError("Topic cannot be null or empty");
        if (string.IsNullOrEmpty(title))
            result.SetError("Title cannot be null or empty");
        if (string.IsNullOrEmpty(body))
            result.SetError("Body cannot be null or empty");

        try
        {
            var message = new Message()
            {
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body,
                    ImageUrl = imageUrl
                },
                Data = data,
                Topic = topic,
            };

            var response = await _messaging.SendAsync(message);
            if (response == null)
            {
                result.SetError("Failed to send notification");
                return result;
            }
            
            if (data == null)
                data = new Dictionary<string, string>();
            
            data["messageId"] = response;
            
            result.SetSuccess($"Notification sent successfully. Message ID: {response}");
        }
        catch (FirebaseException ex)
        {
            result.SetError($"Firebase error: {ex.Message}");
            return result;
        }
        catch (Exception ex)
        {
            result.SetError($"Unexpected error: {ex.Message}");
            return result;
        }
        
        return result;
    }

    public async Task SubscribeToTopicAsync(string deviceToken, string topic)
    {
        if (string.IsNullOrEmpty(deviceToken))
            throw new ArgumentException("Device token cannot be null or empty", nameof(deviceToken));
        if (string.IsNullOrEmpty(topic))
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

        try
        {
            var response = await _messaging.SubscribeToTopicAsync(new List<string> { deviceToken }, topic);
        }
        catch (FirebaseException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task UnsubscribeFromTopicAsync(string deviceToken, string topic)
    {
        if (string.IsNullOrEmpty(deviceToken))
            throw new ArgumentException("Device token cannot be null or empty", nameof(deviceToken));
        if (string.IsNullOrEmpty(topic))
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

        try
        {
            var response = await _messaging.UnsubscribeFromTopicAsync(new List<string> { deviceToken }, topic);
           
        }
        catch (FirebaseException ex)
        {
          
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

public class FirebaseConfig
{
    public string CredentialsPath { get; set; }
}