using AvvaMobile.Core.Business;

namespace Quark.Business.Caching.Interfaces;

public interface IAppSettingsCachingService
{
    Task<ServiceResult> RefreshAll();
}