using AvvaMobile.Core.Business;

namespace Quark.API.Business.Interfaces
{
    public interface IAppSettingsService
    {
        Task<ServiceResult> LoadAllSettingsToCache();
        ServiceResult<Dictionary<string, string>> AppVersions();
    }
}