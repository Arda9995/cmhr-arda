using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.MenuResponses;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.MenuCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.API.Business.Services;

public class MenuService : BaseService, IMenuService
{
    private readonly IMenuCachingService _menuCachingService;
    public MenuService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IMenuCachingService menuCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _menuCachingService = menuCachingService;
    }

    public async Task<ServiceResult<List<MenuCachingResponse>>> ListMenusAsync()
    {
        var result = new ServiceResult<List<MenuCachingResponse>>();

        var menuList = await _menuCachingService.GetAll();

        result.Data = menuList.Data;

        return result;
    }

    public async Task<ServiceResult<List<MenuCachingResponse>>> ListMenusByKeyAsync(string key)
    {
        var result = new ServiceResult<List<MenuCachingResponse>>();

        var menuList = await _menuCachingService.GetAll();

        var flatMenuList = FlattenMenus(menuList.Data);

        result.Data = flatMenuList.Where(x => x.Key == key).ToList();

        return result;
    }

    private List<MenuCachingResponse> FlattenMenus(List<MenuCachingResponse> menus)
    {
        var flatList = new List<MenuCachingResponse>();

        foreach (var menu in menus)
        {
            flatList.Add(menu);

            if (menu.Children != null && menu.Children.Any())
            {
                flatList.AddRange(FlattenMenus(menu.Children)); 
            }
        }

        return flatList;
    }


    public async Task<ServiceResult<ListMenusResponse>> ListMenusByIDAsync(int id)
    {
        var result = new ServiceResult<ListMenusResponse>();

        var menuHierarchy = await _menuCachingService.GetByID(id);

        result.Data = new ListMenusResponse
        {
            ID = menuHierarchy.Data.ID,
            Name = menuHierarchy.Data.Name,
            ParentID = menuHierarchy.Data.ParentID,
            Order = menuHierarchy.Data.Order,
            OpenInNewWindow = menuHierarchy.Data.OpenInNewWindow,
            URL = menuHierarchy.Data.URL,
            Children = menuHierarchy.Data.Children,
        };

        return result;
    }
}