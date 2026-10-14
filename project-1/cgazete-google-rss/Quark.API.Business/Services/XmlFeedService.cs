using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quark.Business.Common.Caching;
using Quark.Core.Extentions;
using Quark.Data;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Resources;

namespace Quark.API.Business.Services.Interfaces
{
	public class XmlFeedService : BaseService, IXmlFeedService
	{
		private readonly QuarkDataContext _context;
		private readonly AppSettingsKeys _appSettingsKeys;
		private readonly IConfiguration _configuration;
		private const int MAX_SITEMAP_ITEMS = 50000;
		private const int MAX_RSS_ITEMS = 50;
		private readonly string _siteUrl;
		private readonly string _rssFilePath;
		private readonly string _sitemapFilePath;
		private readonly XmlWriterSettings _xmlSettings;
		private readonly IXmlFeedCachingService _xmlFeedCachingService;

		public XmlFeedService(
			IHttpContextAccessor httpContext, 
			QuarkDataContext context, 
			IStringLocalizer<CommonResource> localizer, 
			AppSettingsKeys appSettingsKeys, 
			AvvaMobile.Core.Caching.ICacheManager cache, 
			IXmlFeedCachingService xmlFeedCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
		{
			_xmlFeedCachingService = xmlFeedCachingService;
		}

		private string SerializeXml(XDocument doc)
		{
			using var ms = new MemoryStream();
			using (var writer = XmlWriter.Create(ms, _xmlSettings))
			{
				doc.Save(writer);
			}
			return Encoding.UTF8.GetString(ms.ToArray());
		}

		public async Task<string> GetNewsSitemap()
		{
			var cachedContent = await _xmlFeedCachingService.GetNewsSitemap();
			if (string.IsNullOrEmpty(cachedContent))
			{
				await _xmlFeedCachingService.RefreshNewsSiteMap();
				cachedContent = await _xmlFeedCachingService.GetNewsSitemap();
			}
			return cachedContent;
		}

		public async Task<string> GetPostsSitemap()
		{
			var cachedContent = await _xmlFeedCachingService.GetPostsSitemap();
			if (string.IsNullOrEmpty(cachedContent))
			{
				await _xmlFeedCachingService.RefreshPostsSiteMap();
				cachedContent = await _xmlFeedCachingService.GetPostsSitemap();
			}
			return cachedContent;
		}

		public async Task<string> GetMonthlyPostsSitemap(int year, int month)
		{
			var cachedContent = await _xmlFeedCachingService.GetMonthlyPostsSitemap(year, month);
			if (string.IsNullOrEmpty(cachedContent))
			{
				await _xmlFeedCachingService.RefreshMonthlyPostsSitemap(year, month);
				cachedContent = await _xmlFeedCachingService.GetMonthlyPostsSitemap(year, month);
			}
			return cachedContent;
		}

		public async Task<string> GetSourceSitemap()
		{
			var cachedContent = await _xmlFeedCachingService.GetSourcesSitemap();
			if (string.IsNullOrEmpty(cachedContent))
			{
				await _xmlFeedCachingService.RefreshSourcesSiteMap();
				cachedContent = await _xmlFeedCachingService.GetSourcesSitemap();
			}
			return cachedContent;
		}

		public async Task<string> GetTagsSitemap()
		{
			var cachedContent = await _xmlFeedCachingService.GetTagsSitemap();
			if (string.IsNullOrEmpty(cachedContent))
			{
				await _xmlFeedCachingService.RefreshTagsSiteMap();
				cachedContent = await _xmlFeedCachingService.GetTagsSitemap();
			}
			return cachedContent;
		}

		public async Task<string> GetMonthlyTagsSitemap(int year, int month)
		{
			var cachedContent = await _xmlFeedCachingService.GetMonthlyTagsSitemap(year, month);
			if (string.IsNullOrEmpty(cachedContent))
			{
				await _xmlFeedCachingService.RefreshTagsSiteMap();
				cachedContent = await _xmlFeedCachingService.GetMonthlyTagsSitemap(year, month);
			}
			return cachedContent;
		}

		public async Task<string> GetMainRss()
		{
			var cachedContent = await _xmlFeedCachingService.GetMainRSS();
			if (string.IsNullOrEmpty(cachedContent))
			{
				await _xmlFeedCachingService.RefreshMainRSS();
				cachedContent = await _xmlFeedCachingService.GetMainRSS();
			}
			return cachedContent;
		}

		public Task<string> GetCategoryRss(string category)
		{
			return _xmlFeedCachingService.GetCategoryRSS(category);
		}

		public async Task<string> GetGoogleAiRss()
		{
			var cachedContent = await _xmlFeedCachingService.GetGoogleAiRSS();
			if (string.IsNullOrEmpty(cachedContent))
			{
				await _xmlFeedCachingService.RefreshGoogleAiRSS();
				cachedContent = await _xmlFeedCachingService.GetGoogleAiRSS();
			}
			return cachedContent;
		}
	}
}