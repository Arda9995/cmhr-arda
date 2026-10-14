using AvvaMobile.Core.Business;

namespace Quark.Hangfire.Business.Interfaces;

public interface IXmlFeedHangfireService
{
    Task RefreshNewsSiteMap();
    Task RefreshTagsSiteMap();
    Task RefreshPostsSiteMap();
    Task RefreshSourcesSiteMap();
    Task RefreshMainRSS();
    Task RefreshCategoryRSS();
    Task RefreshGoogleAiRSS();
    Task RefreshSiteMaps();
    Task RefreshLast3MonthlyPostsSitemap();
    Task RefreshLast3MonthlyTagsSitemap();

    Task RefreshMonthlyPostsSitemap();
    Task RefreshMonthlyTagsSitemap();
}