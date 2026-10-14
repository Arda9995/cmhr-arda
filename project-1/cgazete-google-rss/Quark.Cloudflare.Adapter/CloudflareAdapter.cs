using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Quark.Business.Common.BaseServices;
using Quark.Cloudflare.Adapter.Interfaces;
using Quark.Cloudflare.Adapter.Models;
using Quark.Data;
using Quark.Resources;
using System.Net;
using System.Text;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Cloudflare.Adapter
{
    public class CloudflareAdapter : BaseService,ICloudflareAdapter
    {
        private string _userEmail;
        private string _apiKey;
        private string _zoneId;

        private string _apiEndpoint = "https://api.cloudflare.com/client/v4";

		public CloudflareAdapter(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
		{
			_zoneId = _appSettingsKeys.CloudflareCache_ZoneID;
			_apiKey = _appSettingsKeys.CloudflareCache_APIKey;
			_userEmail = _appSettingsKeys.CloudflareCache_Email;
		}

		public bool Purge(string url)
        {
            CloudflareResponse purgeResponse = null;

            byte[] data;
            CloudflareFileInfo fileInfo = new CloudflareFileInfo
            {
                Files = new List<string> { url }
            };

            data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(fileInfo));

            HttpWebRequest purgeRequest = WebRequest.CreateHttp($"{_apiEndpoint}/zones/{_zoneId}/purge_cache");
            purgeRequest.Method = "POST";
            purgeRequest.ContentType = "application/json";
            purgeRequest.Headers.Add("X-Auth-Email", _userEmail);
            purgeRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");

            purgeRequest.ContentLength = data.Length;

            try
            {
                using (Stream fileStream = purgeRequest.GetRequestStream())
                {
                    fileStream.Write(data, 0, data.Length);
                    fileStream.Flush();
                }

                using (WebResponse response = purgeRequest.GetResponse())
                {
                    using (StreamReader purgeStream = new StreamReader(response.GetResponseStream()))
                    {
                        string responseJson = purgeStream.ReadToEnd();

                        if (!string.IsNullOrEmpty(responseJson))
                            purgeResponse = JsonConvert.DeserializeObject<CloudflareResponse>(responseJson);
                    }
                }

                return purgeResponse.Success;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
