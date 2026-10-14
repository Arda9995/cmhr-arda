using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.MenuCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;
using System.Linq.Dynamic.Core;
using Quark.Business.Common.Caching;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class MenuCachingService : BaseService, IMenuCachingService
{
    public MenuCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult<MenuCachingResponse>> GetByID(int id)
    {
        var result = new ServiceResult<MenuCachingResponse>();

        var _cachedMenu = await _cache.Get<MenuCachingResponse>(string.Format(RedisKeys.MenuItem, id));

        if (_cachedMenu == null)
        {
            await RefreshSingle(id);
            _cachedMenu = await _cache.Get<MenuCachingResponse>(string.Format(RedisKeys.MenuItem, id));
        }

        result.Data = _cachedMenu;
        return result;
    }

    public async Task<ServiceResult<List<MenuCachingResponse>>> GetAll()
    {
        var result = new ServiceResult<List<MenuCachingResponse>>();

        var _cachedMenu = await _cache.Get<List<MenuCachingResponse>>(RedisKeys.Menus);

        if (_cachedMenu == null)
        {
            await RefreshAll();
            _cachedMenu = await _cache.Get<List<MenuCachingResponse>>(RedisKeys.Menus);
        }

        result.Data = _cachedMenu;
        return result;
    }

    private async Task<ServiceResult<List<MenuCachingResponse>>> GetMenuHierarchy(int parentId)
    {
        var result = new ServiceResult<List<MenuCachingResponse>>();

        var mylist = new List<MenuCachingResponse>();

        var childMenus = await _context.Menus
         .Where(m => m.ParentID == parentId)
         .OrderBy(m => m.Order)
         .ToListAsync();

        foreach (var menu in childMenus)
        {
            var parentMenu = new MenuCachingResponse
            {
                ID = menu.ID,
                Name = menu.Name,
                ParentID = menu.ParentID,
                URL = menu.URL,
                OpenInNewWindow = menu.OpenInNewWindow,
                Order = menu.Order,
                Key = menu.Key,
            };

            var children = await GetMenuHierarchy(menu.ID);
            parentMenu.Children = children.Data;

            mylist.Add(parentMenu);
        }

        result.Data = mylist;
        return result;
    }


    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        var menus = await _context.Menus.AsNoTracking()
            .Where(m => m.ParentID == null && m.IsEnabled)
            .ToListAsync();

        if (menus != null)
        {
            var menusResponse = new List<MenuCachingResponse>();

            foreach (var menu in menus)
            {
                var menuResponse = new MenuCachingResponse
                {
                    ID = menu.ID,
                    Name = menu.Name,
                    Order = menu.Order,
                    ParentID = menu.ParentID,
                    URL = menu.URL,
                    OpenInNewWindow = menu.OpenInNewWindow,
                    Key = menu.Key
                };

                var children = await GetMenuHierarchy(menu.ID);
                menuResponse.Children = children.Data;

                menusResponse.Add(menuResponse);

                await _cache.Set(string.Format(RedisKeys.MenuItem, menu.ID), menuResponse);
            }

            await _cache.Set(RedisKeys.Menus, menusResponse);
        }

        return result;
    }


    public async Task<ServiceResult> RefreshSingle(int id)
    {
        var result = new ServiceResult();

        var parent = await (from b in _context.Menus
                            where b.ID == id && b.IsEnabled
                            select new MenuCachingResponse
                            {
                                ID = b.ID,
                                Name = b.Name,
                                Order = b.Order,
                                ParentID = b.ParentID,
                                URL = b.URL,
                                OpenInNewWindow = b.OpenInNewWindow
                            }).AsNoTracking().FirstOrDefaultAsync();

        var children = await GetMenuHierarchy(id);
        parent.Children = children.Data;

        await _cache.Set(string.Format(RedisKeys.MenuItem, parent.ID), parent);

        return result;
    }


    public async Task<ServiceResult> RemoveSingle(int id)
    {
        var result = new ServiceResult();

        await _cache.Remove(string.Format(RedisKeys.MenuItem, id));

        return result;
    }
}