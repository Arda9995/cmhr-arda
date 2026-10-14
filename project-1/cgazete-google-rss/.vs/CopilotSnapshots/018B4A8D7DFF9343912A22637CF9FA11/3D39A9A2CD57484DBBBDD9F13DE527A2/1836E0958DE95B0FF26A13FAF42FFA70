using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using Elastic.Clients.Elasticsearch.TransformManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.AgencyAdapter;
using Quark.Business.Common.AgencyAdapter.Models;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Data.Models.News;
using Quark.Hangfire.Business.Interfaces;
using Quark.Resources;
using System.Globalization;
using System.Xml.Linq;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Hangfire.Business.Services;

public class AgencyHangfireService(
    QuarkDataContext context,
    IHttpContextAccessor httpContext,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    IAgencydapterService aaAdapterService)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), IAgencyHangfireService
{
    public async Task<List<SearchResultItem>> GetAANews()
    {
        var result = new List<SearchResultItem>();
        var aaCategoryMap = new Dictionary<int, int>
            {
                { 1, 3 },
                { 2, 8 },
                { 3, 5 },
                { 4, 11 },
                { 5, 10 },
                { 6, 2 },
                { 7, 6 }
            };

        for (int i = 1; i <= 7; i++)
        {
            var searchParams = new SearchParameters
            {
                StartDate = DateTime.Now.AddHours(-1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EndDate = "NOW",
                FilterCategory = i.ToString(),
                FilterType = "1",       // Haber
                FilterLanguage = "1",   // Türkçe
            };

            Task.Delay(500).Wait();
            var search = aaAdapterService.SearchAsync(searchParams).Result;
            result.AddRange(search.Data.Result.Select(x => new SearchResultItem
            {
                Id = x.Id,
                Date = x.Date.AddHours(3),
                Title = x.Title,
                Type = x.Type,
                CategoryID = aaCategoryMap[i],
            }));
        }

        XNamespace ns = "http://iptc.org/std/nar/2006-10-01/";
        XNamespace nitf = "http://iptc.org/std/NITF/2006-10-18/";

        await Task.Delay(2500);

        foreach (var item in result)
        {
            // Check if the news already exists in the database
            if (_context.AgencyNews.Any(x => x.AgencyNewsID == item.Id))
            {
                continue; // Skip if the news already exists
            }

            // If it doesn't exist, create a new AgencyNews object and add it to the context
            var docParams = new DocumentParameters
            {
                Id = item.Id,
                Format = "newsml29"
            };

            await Task.Delay(2500);
            string xml = aaAdapterService.DocumentRawAsync(docParams).Result;
            var document = XDocument.Parse(xml);

            string content = document.Descendants(nitf + "body.content").FirstOrDefault()?.Value;
            string subtitle = document.Descendants(nitf + "abstract").FirstOrDefault()?.Value;

            var agencyNews = new AgencyNews
            {
                AgencyNewsID = item.Id,
                AgencyID = 2, // AA ya denk geliyor db de.
                CategoryID = item.CategoryID,
                Date = item.Date,
                Title = item.Title,
                Type = item.Type,
                Content = content,
                Subtitle = string.IsNullOrWhiteSpace(subtitle) ? item.Title : subtitle
            };

            _context.AgencyNews.Add(agencyNews);
            _context.SaveChanges();
        }

        return result;
    }

    public async Task<List<NewsItem>> GetRssItemsAsync()
    {
        var result = new List<NewsItem>();

        var rsslinks = await _context.RssLinks
            .Where(x => x.IsActive)
            .ToListAsync();
        foreach (var rssLink in rsslinks)
        {
            var data = await aaAdapterService.GetRssItemsAsync(rssLink);

            foreach (var item in data)
            {
                if (_context.AgencyNews.Any(x => x.AgencyNewsID == item.AgencyNewsID))
                {
                    continue; // Skip if the news already exists
                }

                var agencyNews = new AgencyNews
                {
                    AgencyNewsID = item.AgencyNewsID,
                    AgencyID = rssLink.AgencyID, // AA ya denk geliyor db de.
                    CategoryID = rssLink.CategoryID ?? item.CMSCategoryID,
                    Date = ParseDateString(item.PubDate),
                    Title = item.Title,
                    Type = "text",
                    Content = item.Description,
                    Subtitle = item.Title
                };

                _context.AgencyNews.Add(agencyNews);
                _context.SaveChanges();
            }
        }

        return result;
    }

    private DateTime ParseDateString(string date)
    {
        if (DateTime.TryParseExact(
        date,
        "yyyy-MM-dd HH:mm:ss",
        System.Globalization.CultureInfo.InvariantCulture,
        System.Globalization.DateTimeStyles.None,
        out DateTime result))
        {
            return result;
        }
        else
        {
           return DateTime.ParseExact(date, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}