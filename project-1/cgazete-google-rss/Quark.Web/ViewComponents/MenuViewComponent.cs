using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.Web.Models.ViewComponentResponses;

namespace Quark.Web.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;
        public MenuViewComponent(IMenuService menuService)
        {
            _menuService = menuService;
        }


        public async Task<IViewComponentResult> InvokeAsync(string methodName)
        {
            bool isMobile = Request.Headers["User-Agent"].ToString().ToLower().Contains("mobile");

            switch (methodName)
            {
                case "Hamburger_KategoriMenu":
                    var hamburgerMenu = await GetMenuList(methodName);
                    return View($"~/Views/Shared/Components/Menu/{methodName}.cshtml", hamburgerMenu);
				case "Desktop_HamburgerMenu":
					var desktopHamburgerMenu = await GetMenuList(methodName);
					return View($"~/Views/Shared/Components/Menu/{methodName}.cshtml", desktopHamburgerMenu);

				case "MobileHeader_HamburgerMenu":
                case "MobileFooter_Kolon1":
                case "MobileFooter_Kolon2":
                case "MobileFooter_Kolon3":
                    var mobileMenuList = await GetMenuList(methodName);
                    return View($"~/Views/Shared/Components/MobileMenu/{methodName}.cshtml", mobileMenuList);

                case "MobileFooter_KolonAll":
                    var mobileFooterKolon1 = await GetMenuList("MobileFooter_Kolon1");
                    var mobileFooterKolon2 = await GetMenuList("MobileFooter_Kolon2");
                    var mobileFooterKolon3 = await GetMenuList("MobileFooter_Kolon3");

                    var mobileListAllFooterMenus = new List<MenuItems>();
                    mobileListAllFooterMenus.AddRange(mobileFooterKolon1);
                    mobileListAllFooterMenus.AddRange(mobileFooterKolon2);
                    mobileListAllFooterMenus.AddRange(mobileFooterKolon3);

                    return View($"~/Views/Shared/Components/MobileMenu/{methodName}.cshtml", mobileListAllFooterMenus);

                case "Header_MenuUst":
                case "Header_MenuAlt":
                case "Footer_Kolon1":
                case "Footer_Kolon2":
                case "Footer_Kolon3":
                    var menuList = await GetMenuList(methodName);

                    if (isMobile)
                        return View($"~/Views/Shared/Components/MobileMenu/{methodName}.cshtml", menuList);
                    else
                        return View($"~/Views/Shared/Components/Menu/{methodName}.cshtml", menuList);

                case "Footer_KolonAll":
                    var footerKolon1 = await GetMenuList("Footer_Kolon1");
                    var footerKolon2 = await GetMenuList("Footer_Kolon2");
                    var footerKolon3 = await GetMenuList("Footer_Kolon3");

                    var listAllFooterMenus = new List<MenuItems>();
                    listAllFooterMenus.AddRange(footerKolon1);
                    listAllFooterMenus.AddRange(footerKolon2);
                    listAllFooterMenus.AddRange(footerKolon3);

                    if (isMobile)
                        return View($"~/Views/Shared/Components/MobileMenu/{methodName}.cshtml", listAllFooterMenus);
                    else
                        return View($"~/Views/Shared/Components/Menu/{methodName}.cshtml", listAllFooterMenus);

                default:
                    return View();
            }
        }

        private async Task<List<MenuItems>> GetMenuList(string key)
        {
            var menuItems = await _menuService.ListMenusAsync();
            var menuModelList = menuItems.Data.Where(x => x.Key == key).FirstOrDefault().Children.OrderBy(x => x.Order).Select(x => new MenuItems
            {
                Key = x.Key,
                Name = x.Name,
                URL = x.URL,
                OpenInNewWindow = x.OpenInNewWindow,
                Order = x.Order,
                Children = x.Children,
                ID = x.ID,
                ParentID = x.ParentID
            }).ToList();
            return menuModelList;
        }
    }
}
