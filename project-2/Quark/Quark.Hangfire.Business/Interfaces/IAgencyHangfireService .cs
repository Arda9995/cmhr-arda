using Quark.Business.Common.AgencyAdapter.Models;

namespace Quark.Hangfire.Business.Interfaces
{
    public interface IAgencyHangfireService
    {
        Task<List<SearchResultItem>> GetAANews();
        Task<List<NewsItem>> GetRssItemsAsync();
    }
}