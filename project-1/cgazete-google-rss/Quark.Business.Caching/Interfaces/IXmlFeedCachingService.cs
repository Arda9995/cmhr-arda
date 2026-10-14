using AvvaMobile.Core.Business;

namespace Quark.Business.Caching.Interfaces
{
	public interface IXmlFeedCachingService
	{

		#region Refresh Methods

		Task<ServiceResult> RefreshAllRSS();
		Task<ServiceResult> RefreshAllSiteMaps();
		Task<ServiceResult> RefreshNewsSiteMap();
		Task<ServiceResult> RefreshTagsSiteMap();
		Task<ServiceResult> RefreshPostsSiteMap();
		Task<ServiceResult> RefreshSourcesSiteMap();
		Task<ServiceResult> RefreshMainRSS();
		Task<ServiceResult> RefreshCategoryRSS(string category);
		Task<ServiceResult> RefreshGoogleAiRSS();
		Task<ServiceResult> RemoveGoogleAiNewsFromFeedAsync(long newsId);
		Task<ServiceResult<string>> ArchiveGoogleAiNewsAsync(long newsId);
		Task<ServiceResult<string>> RefreshGoogleAiArchiveAsync();
		Task<ServiceResult> RefreshSiteMaps();
		Task<ServiceResult> RefreshMonthlyPostsSitemap(int year, int month);
		Task<ServiceResult> RefreshMonthlyTagsSitemap(int year, int month);

		#endregion

		#region Get Methods

		Task<string> GetNewsSitemap();
		Task<string> GetPostsSitemap();
		Task<string> GetTagsSitemap();
		Task<string> GetSourcesSitemap();
		Task<string> GetMainRSS();
		Task<string> GetCategoryRSS(string category);
		Task<string> GetGoogleAiRSS();
		Task<string> GetMonthlyPostsSitemap(int year, int month);
		Task<string> GetMonthlyTagsSitemap(int year, int month);

		#endregion
	}
}
