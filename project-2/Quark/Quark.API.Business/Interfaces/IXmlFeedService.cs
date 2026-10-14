namespace Quark.API.Business.Services.Interfaces
{
    public interface IXmlFeedService
    {
        Task<string> GetNewsSitemap();
        Task<string> GetPostsSitemap();
        Task<string> GetMonthlyPostsSitemap(int year, int month);
        Task<string> GetSourceSitemap();
        Task<string> GetTagsSitemap();
        Task<string> GetMonthlyTagsSitemap(int year, int month);
        Task<string> GetMainRss();
        Task<string> GetCategoryRss(string category);
    }
} 