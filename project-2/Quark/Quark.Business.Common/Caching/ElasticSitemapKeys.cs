namespace Quark.Business.Common.Caching;

public class ElasticSitemapKeys
{
    public static string NewsSitemapKey => "NewsSitemap";
    public static string PostsSitemapKey => "PostsSitemap";
    public static string TagsSitemapKey => "TagsSitemap";
    public static string SourceSitemapKey => "SourceSitemap";
    public static string MonthlyPostsSitemapKey(int year, int month) => $"MonthlyPostsSitemap_{year}_{month}";
    public static string MonthlyTagsSitemapKey(int year, int month) => $"MonthlyTagsSitemap_{year}_{month}";
}

public enum ElasticSitemapKeysEnum
{
    NewsSitemapKey = 1,
    PostsSitemapKey,
    TagsSitemapKey,
    SourceSitemapKey
}

