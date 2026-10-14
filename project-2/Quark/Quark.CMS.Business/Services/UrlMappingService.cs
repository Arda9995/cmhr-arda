using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.Data;
using Quark.Resources;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Quark.CMS.Business.Services
{

	public class UrlMappingService : BaseService, IUrlMappingService
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public UrlMappingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, IHttpClientFactory httpClientFactory) : base(httpContext, context, localizer, appSettingsKeys, cache)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task NotifyUrlMappingChanged()
		{
			await SendNotification("refresh-url-mappings");
		}

		private async Task SendNotification(string endpoint)
		{
			try
			{
				var webSiteUrl = _appSettingsKeys.Common_ClientWebSite;
				if (string.IsNullOrEmpty(webSiteUrl))
				{
					return;
				}

				var client = _httpClientFactory.CreateClient();
				var url = webSiteUrl.TrimEnd('/') + "/UrlMapping/" + endpoint;

				var response = await client.PostAsync(url, null);
				if (!response.IsSuccessStatusCode)
				{
					var error = await response.Content.ReadAsStringAsync();
				}
			}
			catch (Exception ex)
			{
			}
		}
	}
}