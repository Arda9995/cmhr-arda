using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Notification.Adapter.Models.Requests;
using Quark.Notification.Adapter.Models.Responses;
using Quark.Resources;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Quark.Notification.Adapter
{
	public class NetmeraAdapter : BaseService
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;
		private readonly string _baseUrl;

		public NetmeraAdapter(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, HttpClient httpClient) : base(httpContext, context, localizer, appSettingsKeys, cache)
		{
			_httpClient = httpClient;
			_apiKey = _appSettingsKeys.Netmera_APIKey;
			_baseUrl = _appSettingsKeys.Netmera_APIURL;

			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.Add("X-netmera-api-key", _apiKey);
		}


		public async Task<ServiceResult<NetmeraNotificationResponse>> SendNotificationAsync(NetmeraNotificationRequest request)
		{
			var result = new ServiceResult<NetmeraNotificationResponse>();

			try
			{
				var json = JsonSerializer.Serialize(request);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync($"{_baseUrl}/sendBulkNotification", content);
				var responseContent = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					var notificationResponse = JsonSerializer.Deserialize<NetmeraNotificationResponse>(responseContent);
					result.Data = notificationResponse;
				}
				else
				{
					result.SetError($"Failed to send notification. Status code: {response.StatusCode}, Error: {responseContent}");
				}
			}
			catch (Exception ex)
			{
				result.SetError($"Exception occurred while sending notification: {ex.Message}");
			}

			return result;
		}


		public async Task<ServiceResult<NetmeraNotificationResponse>> SendWebPushNotificationAsync(NetmeraNotificationRequest request)
		{
			var result = new ServiceResult<NetmeraNotificationResponse>();

			if (string.IsNullOrEmpty(request.Message.Title))
			{
				result.SetError("Notification title cannot be empty");
				return result;
			}

			if (string.IsNullOrEmpty(request.Message.Text))
			{
				result.SetError("Notification message cannot be empty");
				return result;
			}

			if (string.IsNullOrEmpty(request.Message.IconUrl))
			{
				result.SetError("Icon URL cannot be empty for web push notifications");
				return result;
			}

			var requestModel = new NetmeraNotificationRequest
			{
				Message = new NetmeraMessage
				{
					Title = request.Message.Title,
					Text = request.Message.Text,
					IconUrl = request.Message.IconUrl,
					WpChromeImage = request.Message.WpChromeImage,
					InteractionRequired = request.Message.InteractionRequired,
					Platforms = new List<string> { "CHROME", "MACOS", "FIREFOX", "OPERA", "EDGE", "YANDEX" },
					Click = new NetmeraClick
					{
						Url = request.Message.Click.Url
					}
				},

				Target = new NetmeraTarget
				{
					SendToAll = true,
				},
			};

			return await SendNotificationAsync(request);
		}
	}
}
