using AvvaMobile.Core.Business;

namespace Quark.Hangfire.Business.Interfaces;

public interface IXmlFeedHangfireService
{
    void RefreshNewsSiteMap();
    void RefreshTagsSiteMap();
    void RefreshPostsSiteMap();
    void RefreshSourcesSiteMap();
    void RefreshMainRSS();
    void RefreshCategoryRSS();
    void RefreshSiteMaps();
    void RefreshLast3MonthlyPostsSitemap();
    void RefreshLast3MonthlyTagsSitemap();
    
    void RefreshMonthlyPostsSitemap();
    void RefreshMonthlyTagsSitemap();
}