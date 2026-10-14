using AvvaMobile.Core.Business;
using Quark.Data.Models;

namespace Quark.CMS.Business.Interfaces;

public interface IAppSettingsService
{
    Task<ServiceResult<List<AppSettingsGroup>>> Update();
        
    Task<ServiceResult> Update(List<AppSettingsGroup> request);
}