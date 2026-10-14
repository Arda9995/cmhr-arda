using System.Globalization;
using System.Reflection;
using System.Security;
using System.Xml.Linq;
using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Quark.Business.Caching.Models.Responses.XmlFeedCachingResponses;
using Quark.Business.Caching.Services;
using Quark.Business.Common.Caching;
using Quark.Business.ElasticSearch.Abstractions;
using Quark.Business.ElasticSearch.Constants;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

internal static class XmlFeedRefactorChecks
{
    private static MethodInfo Method(string name) => typeof(XmlFeedCachingService).GetMethod(name,
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        ?? throw new InvalidOperationException($"Missing XML helper: {name}");

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private static string DateText(DateTime date)
    {
        var value = new DateTimeOffset(date, TimeZoneInfo.Local.GetUtcOffset(date))
            .ToString("ddd, dd MMM yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture);
        return value.Remove(value.LastIndexOf(':'), 1);
    }

    public static async Task RunAsync()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddScoped<ICacheManager, MemoryCacheManager>();
        services.AddLocalization();
        services.AddLogging();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<ICacheManager>();
        await cache.Set("Common_ClientWebSite", "https://example.test/");
        await using var context = new QuarkDataContext(new DbContextOptionsBuilder<QuarkDataContext>().Options);
        var elastic = DispatchProxy.Create<IElasticSearchService, XmlFeedCheckElasticProxy>();
        var proxy = (XmlFeedCheckElasticProxy)(object)elastic;
        var service = new XmlFeedCachingService(
            new HttpContextAccessor { HttpContext = new DefaultHttpContext() }, context,
            scope.ServiceProvider.GetRequiredService<IStringLocalizer<CommonResource>>(),
            new AppSettingsKeys(cache), cache, elastic,
            scope.ServiceProvider.GetRequiredService<ILogger<XmlFeedCachingService>>());

        await CheckCacheAsync(service, proxy);
        CheckRss(service);
        CheckSitemapSchedule();
        Console.WriteLine("All XML refactor checks passed. No database, Elasticsearch or archive files were accessed.");
    }

    private static async Task CheckCacheAsync(XmlFeedCachingService service, XmlFeedCheckElasticProxy proxy)
    {
        var getters = new (Func<Task<string>> Get, string Id, string Index)[]
        {
            (service.GetMainRSS, ((int)ElasticRssKeysEnum.MainRssKey).ToString(), IndexNames.MainRSS),
            (service.GetGoogleAiRSS, ((int)ElasticRssKeysEnum.GoogleAiRssKey).ToString(), IndexNames.GoogleAiRSS),
            (service.GetNewsSitemap, ((int)ElasticSitemapKeysEnum.NewsSitemapKey).ToString(), IndexNames.Sitemaps),
            (service.GetPostsSitemap, ((int)ElasticSitemapKeysEnum.PostsSitemapKey).ToString(), IndexNames.Sitemaps),
            (service.GetTagsSitemap, ((int)ElasticSitemapKeysEnum.TagsSitemapKey).ToString(), IndexNames.Sitemaps),
            (service.GetSourcesSitemap, ((int)ElasticSitemapKeysEnum.SourceSitemapKey).ToString(), IndexNames.Sitemaps),
            (() => service.GetMonthlyPostsSitemap(2026, 9), "2026-9", IndexNames.Monthly_Post_Sitemap),
            (() => service.GetMonthlyTagsSitemap(2026, 9), "2026-9", IndexNames.Monthly_Tag_Sitemap)
        };
        foreach (var sample in getters)
        {
            proxy.Reset(new SitemapRssCachingResponse { Content = "cached" });
            Check(await sample.Get() == "cached" && proxy.Reads.SequenceEqual(new[] { (sample.Id, sample.Index) }),
                "Public getter changed its cache key, index or hit behavior.");
        }

        var read = Method("GetOrRefreshXmlAsync");
        SitemapRssCachingResponse Cached(string content) => new() { Content = content };
        var cases = new (SitemapRssCachingResponse First, SitemapRssCachingResponse Second, string Expected, bool Refresh)[]
        {
            (Cached("cached"), null, "cached", false),
            (Cached(" \t"), null, " \t", false),
            (null, Cached("refreshed"), "refreshed", true),
            (Cached(null), Cached("refreshed"), "refreshed", true),
            (Cached(string.Empty), Cached("refreshed"), "refreshed", true),
            (null, null, string.Empty, true),
            (null, Cached(null), string.Empty, true),
            (null, Cached(string.Empty), string.Empty, true)
        };
        foreach (var sample in cases)
        {
            proxy.Reset(sample.First, sample.Second);
            var refreshCount = 0;
            Func<Task<ServiceResult>> refresh = () =>
            {
                refreshCount++;
                var result = new ServiceResult();
                result.SetError("Refresh result does not suppress the original second cache read.");
                return Task.FromResult(result);
            };
            var actual = await (Task<string>)read.Invoke(service, new object[] { "id", "index", refresh });
            Check(actual == sample.Expected && refreshCount == (sample.Refresh ? 1 : 0) &&
                  proxy.Reads.Count == (sample.Refresh ? 2 : 1) && proxy.Reads.All(x => x == ("id", "index")),
                "Read-through cache semantics changed.");
        }

        var failure = new InvalidOperationException("Expected test failure");
        proxy.Reset((SitemapRssCachingResponse)null);
        Func<Task<ServiceResult>> failingRefresh = () => Task.FromException<ServiceResult>(failure);
        try
        {
            await (Task<string>)read.Invoke(service, new object[] { "id", "index", failingRefresh });
            throw new InvalidOperationException("Refresh exception was swallowed.");
        }
        catch (InvalidOperationException ex) when (ReferenceEquals(ex, failure))
        {
            Check(proxy.Reads.Count == 1, "A throwing refresh must not read again.");
        }

        proxy.Reset();
        proxy.Results.Enqueue(Task.FromException<SitemapRssCachingResponse>(failure));
        try
        {
            await service.GetMainRSS();
            throw new InvalidOperationException("Cache-read exception was swallowed.");
        }
        catch (InvalidOperationException ex) when (ReferenceEquals(ex, failure))
        {
            Check(proxy.Reads.Count == 1, "A throwing read must not invoke a refresh.");
        }

        var xml = new XDocument(new XElement("rss", new XElement("content", new XCData("<p>Türkçe & text</p>"))));
        foreach (var success in new[] { true, false })
        {
            proxy.UpdateResult = success;
            var actual = await (Task<bool>)Method("CacheXmlAsync").Invoke(service,
                new object[] { xml, "document-id", "cache-key", "index-name" });
            Check(actual == success && proxy.Written.ID == "document-id" && proxy.Written.Key == "cache-key" &&
                  proxy.WriteTarget == ("document-id", "index-name"), "Cache write metadata or boolean result changed.");
            Check(proxy.Written.Content.StartsWith("\uFEFF<?xml version=\"1.0\" encoding=\"utf-8\"?>") &&
                  XNode.DeepEquals(xml, XDocument.Parse(proxy.Written.Content.TrimStart('\uFEFF'))),
                "XML declaration, UTF-8 preamble or serialized content changed.");
        }
        Console.WriteLine("PASS cache checks: 8 public getters, 8 read-through cases, 2 exception cases and 2 write-result cases.");
    }

    private static void CheckRss(XmlFeedCachingService service)
    {
        var date = new DateTime(2026, 9, 10, 10, 0, 0);
        var publicationDate = date.AddSeconds(-1);
        var fixtures = new[]
        {
            (Title: "Cumhuriyet Gazetesi", Link: "https://example.test/", Description: "Cumhuriyet Gazetesi", Language: "tr-TR", Self: "https://example.test/rss"),
            (Title: "Cumhuriyet Gazetesi - Kültür & Sanat", Link: "https://example.test/kultur", Description: "Cumhuriyet Gazetesi", Language: "tr-TR", Self: "https://example.test/rss/kultur"),
            (Title: "Cumhuriyet Gazetesi - Google AI Feed", Link: "https://example.test/", Description: "Cumhuriyet Gazetesi Google News AI Pilot Feed", Language: "tr", Self: "https://www.cumhuriyet.com.tr/feeds/google-ai.xml")
        };
        foreach (var sample in fixtures)
        {
            var actual = (XDocument)Method("CreateRssDocument").Invoke(service,
                new object[] { sample.Title, sample.Link, sample.Description, sample.Language, sample.Self, date, publicationDate });
            var expected = XDocument.Parse($"""
                <rss version="2.0" xmlns:content="http://purl.org/rss/1.0/modules/content/" xmlns:media="http://search.yahoo.com/mrss/" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:dc="http://purl.org/dc/elements/1.1/">
                  <channel>
                    <title>{SecurityElement.Escape(sample.Title)}</title>
                    <link>{sample.Link}</link>
                    <description>{sample.Description}</description>
                    <language>{sample.Language}</language>
                    <category>News</category>
                    <lastBuildDate>{DateText(date)}</lastBuildDate>
                    <pubDate>{DateText(publicationDate)}</pubDate>
                    <copyright>Cumhuriyet Gazetesi</copyright>
                    <ttl>60</ttl>
                    <image>
                      <url>https://media.cumhuriyet.com.tr/cumhuriyet-amp-logo.png</url>
                      <title>{SecurityElement.Escape(sample.Title)}</title>
                      <link>{sample.Link}</link>
                      <width>144</width><height>60</height>
                    </image>
                    <atom:link href="{sample.Self}" rel="self" type="application/rss+xml" />
                  </channel>
                </rss>
                """);
            Check(XNode.DeepEquals(expected, actual) && actual.Declaration.Version == "1.0" &&
                  actual.Declaration.Encoding == "utf-8", "RSS channel structure or feed metadata changed.");
        }

        XNamespace media = "http://search.yahoo.com/mrss/";
        XNamespace dc = "http://purl.org/dc/elements/1.1/";
        XNamespace content = "http://purl.org/rss/1.0/modules/content/";
        foreach (var google in new[] { false, true })
        {
            var height = google ? "1200" : "720";
            var width = google ? "1600" : "1280";
            var link = google ? "https://example.test/article" : "https://example.test//article";
            object[] extensions = google
                ? new object[] { new XElement(dc + "creator", "Ayşe Editör"), new XElement("description", "Summary & text"), new XElement(content + "encoded", new XCData("<p>Article</p>")) }
                : new object[] { new XElement("description", new XCData("<p>Summary & text</p>")) };
            var actualItem = (XElement)Method("CreateRssItem").Invoke(service,
                new object[] { "News & title", link, "https://example.test/article", date, "https://example.test/image.jpg", height, width, extensions });
            var actual = new XElement("rss", new XAttribute(XNamespace.Xmlns + "media", media.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "dc", dc.NamespaceName), new XAttribute(XNamespace.Xmlns + "content", content.NamespaceName), actualItem);
            var expectedExtensions = google
                ? "<dc:creator>Ayşe Editör</dc:creator><description>Summary &amp; text</description><content:encoded><![CDATA[<p>Article</p>]]></content:encoded>"
                : "<description><![CDATA[<p>Summary & text</p>]]></description>";
            var expected = XElement.Parse($"""
                <rss xmlns:media="http://search.yahoo.com/mrss/" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:content="http://purl.org/rss/1.0/modules/content/">
                  <item>
                    <title>News &amp; title</title><link>{link}</link>
                    <enclosure url="https://example.test/image.jpg" length="50000" type="image/jpeg" />
                    <media:content url="https://example.test/image.jpg" type="image/jpeg" height="{height}" width="{width}" expression="full" />
                    <guid isPermaLink="false">https://example.test/article</guid>
                    <pubDate>{DateText(date)}</pubDate>
                    {expectedExtensions}
                  </item>
                </rss>
                """);
            Check(XNode.DeepEquals(expected, actual), "RSS item order, URLs, dimensions or extension content changed.");
        }
        Console.WriteLine("PASS RSS fixtures: 3 channel variants and standard/Google AI item structure, CDATA and creator ordering.");
    }

    private static void CheckSitemapSchedule()
    {
        var cases = new (TimeSpan? Age, string Frequency, string Priority)[]
        {
            (TimeSpan.FromHours(-1), "always", "1.0"), (TimeSpan.Zero, "always", "1.0"),
            (TimeSpan.FromHours(24), "always", "1.0"), (TimeSpan.FromHours(24).Add(TimeSpan.FromSeconds(1)), "hourly", "0.9"),
            (TimeSpan.FromHours(48), "hourly", "0.9"), (TimeSpan.FromHours(48).Add(TimeSpan.FromSeconds(1)), "daily", "0.8"),
            (TimeSpan.FromDays(7), "daily", "0.8"), (TimeSpan.FromDays(7).Add(TimeSpan.FromSeconds(1)), "weekly", "0.7"),
            (TimeSpan.FromDays(30), "weekly", "0.7"), (TimeSpan.FromDays(30).Add(TimeSpan.FromSeconds(1)), "monthly", "0.6"),
            (TimeSpan.FromDays(365), "monthly", "0.6"), (TimeSpan.FromDays(365).Add(TimeSpan.FromSeconds(1)), "yearly", "0.5"),
            (null, "yearly", "0.5")
        };
        foreach (var sample in cases)
        {
            var actual = ((string Frequency, string Priority))Method("GetSitemapSchedule").Invoke(null, new object[] { sample.Age });
            Check(actual == (sample.Frequency, sample.Priority), "Sitemap scheduling threshold changed.");
        }
        Console.WriteLine($"PASS {cases.Length} sitemap schedule cases, including exact boundaries, future dates and null.");
    }
}

public class XmlFeedCheckElasticProxy : DispatchProxy
{
    public Queue<Task<SitemapRssCachingResponse>> Results { get; } = new();
    public List<(string Id, string Index)> Reads { get; } = new();
    public bool UpdateResult { get; set; }
    public SitemapRssCachingResponse Written { get; private set; }
    public (string Id, string Index) WriteTarget { get; private set; }

    public void Reset(params SitemapRssCachingResponse[] results)
    {
        Results.Clear();
        Reads.Clear();
        foreach (var result in results)
            Results.Enqueue(Task.FromResult(result));
    }

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        if (targetMethod.Name == nameof(IElasticSearchService.GetDocumentAsync))
        {
            Reads.Add(((string)args[0], (string)args[1]));
            return Results.Dequeue();
        }
        if (targetMethod.Name == nameof(IElasticSearchService.UpdateDocumentAsync))
        {
            Written = (SitemapRssCachingResponse)args[0];
            WriteTarget = ((string)args[1], (string)args[2]);
            return Task.FromResult(UpdateResult);
        }
        throw new InvalidOperationException($"Unexpected Elasticsearch operation: {targetMethod.Name}");
    }
}
