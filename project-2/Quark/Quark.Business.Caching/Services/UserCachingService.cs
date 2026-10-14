using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class UserCachingService : BaseService, IUserCachingService
{

    public UserCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();
        
        var users = await (from u in _context.Users
            select new SelectListItem
            {
                Value = u.ID.ToString(),
                Text = u.FullName
            }).ToListAsync();
        await _cache.Set_SelectListItems(RedisKeys.Users, users);

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> Users()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = await _cache.Get<List<SelectListItem>>(RedisKeys.Users);
        if (result.Data == null)
        {
            await RefreshAll();
            result.Data = await _cache.Get<List<SelectListItem>>(RedisKeys.Users);
        }

        return result;
    }
}