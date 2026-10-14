namespace Quark.Business.Common.Caching;

public class ElasticRssKeys
{
    public static string MainRssKey => "MainRss";
    public static string CategoryRssKey => "CategoryRssKey_{0}";
}

public enum ElasticRssKeysEnum
{
    MainRssKey = 1,
}
