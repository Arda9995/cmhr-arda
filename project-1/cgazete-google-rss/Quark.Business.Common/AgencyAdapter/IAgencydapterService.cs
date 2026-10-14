using Quark.Business.Common.AgencyAdapter.Models;
using Quark.Data.Models;

namespace Quark.Business.Common.AgencyAdapter
{
    public interface IAgencydapterService
    {
        Task<ApiResponse<SearchData>> SearchAsync(SearchParameters parameters);
        Task<string> DocumentRawAsync(DocumentParameters parameters);
        Task<List<NewsItem>> GetRssItemsAsync(RssLink rssLink);
    }
}
