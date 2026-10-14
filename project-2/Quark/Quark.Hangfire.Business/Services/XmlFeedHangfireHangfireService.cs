using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Hangfire.Business.Interfaces;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Hangfire.Business.Services;

public class XmlFeedHangfireHangfireService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    IXmlFeedCachingService xmlFeedCachingService)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), IXmlFeedHangfireService
{
    public void RefreshNewsSiteMap()
    {
        var result = xmlFeedCachingService.RefreshNewsSiteMap().Result;
        //BackgroundJob.Enqueue<IXmlFeedHangfireService>(x => x.RefreshNewsSiteMap());
    }

    public void RefreshTagsSiteMap()
    {
        var result = xmlFeedCachingService.RefreshTagsSiteMap().Result;
        //BackgroundJob.Enqueue<IXmlFeedHangfireService>(x => x.RefreshTagsSiteMap());
    }

    public void RefreshPostsSiteMap()
    {
        var result = xmlFeedCachingService.RefreshPostsSiteMap().Result;
        //BackgroundJob.Enqueue<IXmlFeedHangfireService>(x => x.RefreshPostsSiteMap());
    }

    public void RefreshSourcesSiteMap()
    {
        var result = xmlFeedCachingService.RefreshSourcesSiteMap().Result;
        //BackgroundJob.Enqueue<IXmlFeedHangfireService>(x => x.RefreshSourcesSiteMap());
    }

    public void RefreshMainRSS()
    {
        var result = xmlFeedCachingService.RefreshMainRSS().Result;
        //BackgroundJob.Enqueue<IXmlFeedHangfireService>(x => x.RefreshMainRSS());
    }

    public void RefreshCategoryRSS()
    {
        var categories = _context.Categories.ToList();
        foreach (var category in categories)
        {
            var result = xmlFeedCachingService.RefreshCategoryRSS(category.Url).Result;
        }
        //BackgroundJob.Enqueue<IXmlFeedHangfireService>(x => x.RefreshCategoryRSS());
    }

    public void RefreshSiteMaps()
    {
        var result = xmlFeedCachingService.RefreshSiteMaps().Result;
        //BackgroundJob.Enqueue<IXmlFeedHangfireService>(x => x.RefreshSiteMaps());
    }

    public void RefreshLast3MonthlyPostsSitemap()
    {
        List<int> last3Months = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            last3Months.Add(DateTime.Now.AddMonths(-i).Month);
        }
        
        int currentYear = DateTime.Now.Year;

        foreach (var month in last3Months)
        {
            var result = xmlFeedCachingService.RefreshMonthlyPostsSitemap(currentYear, month).Result;
        }
        //BackgroundJob.Enqueue<IXmlFeedHangfireService>(x => x.RefreshLast3MonthlyPostsSitemap());
    }

    public void RefreshLast3MonthlyTagsSitemap()
    {
        List<int> last3Months = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            last3Months.Add(DateTime.Now.AddMonths(-i).Month);
        }
        
        int currentYear = DateTime.Now.Year;

        foreach (var month in last3Months)
        {
            var result = xmlFeedCachingService.RefreshMonthlyTagsSitemap(currentYear,month).Result;
        }
        //BackgroundJob.Enqueue<IXmlFeedHangfireService>(x => x.RefreshLast3MonthlyTagsSitemap());
    }
    
    public void RefreshMonthlyPostsSitemap()
    {
        var minPublishedDate = _context.NewsPublished.Min(n => n.PublishedDate);

        while (minPublishedDate < DateTime.Now)
        {
            var result = xmlFeedCachingService.RefreshMonthlyPostsSitemap(minPublishedDate.Year, minPublishedDate.Month).Result;
            minPublishedDate = minPublishedDate.AddMonths(1);
        }
    }

    public void RefreshMonthlyTagsSitemap()
    {
        var minCreatedDate = _context.Tags.Min(n => n.CreatedDate) ?? DateTime.Now;

        while (minCreatedDate < DateTime.Now)
        {
            var result = xmlFeedCachingService.RefreshMonthlyTagsSitemap(minCreatedDate.Year, minCreatedDate.Month).Result;
            minCreatedDate = minCreatedDate.AddMonths(1);
        }
    }
}