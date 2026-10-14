using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.MenuRequests;
using Quark.CMS.Business.Models.Responses.MenuResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;



namespace Quark.CMS.Business.Services;

public class MenuService : BaseService, IMenuService
{
	private readonly IMenuCachingService _menuCachingService;
	public MenuService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IMenuCachingService menuCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
	{
		_menuCachingService = menuCachingService;
	}

	public async Task<ServiceResult> ChangeMenuStatus(int ID, bool isEnabled)
	{
		var result = new ServiceResult();

		var menus = _context.Menus.Where(x => x.ID == ID).ToList();
		if (!menus.Any())
		{
			result.SetError(_localizer["RecordNotFound"]);
			return result;
		}

		foreach (var menu in menus)
		{
			menu.IsEnabled = isEnabled;
			_context.Menus.Update(menu);
		}

		if (!menus[0].ParentID.HasValue)
		{
			var childMenus = _context.Menus.Where(x => x.ParentID == menus[0].ID).ToList();
			foreach (var menu in childMenus)
			{
				menu.IsEnabled = isEnabled;
				_context.Menus.Update(menu);
			}
		}

		await _context.SaveChangesAsync();

		result.SetSuccess(_localizer["RecordUpdated"]);
		return result;
	}

	public async Task<ServiceResult<CreateUpdateRequest>> Create(CreateUpdateRequest request)
	{
		var result = new ServiceResult<CreateUpdateRequest> { Data = request };

		var exist = await _context.Menus.Where(x => x.Name == request.Name && x.ParentID == request.ParentID).AnyAsync();
		if (exist)
		{
			result.SetError(_localizer["RecordAlreadyExist"]);
			return result;
		}
		var menu = new Menu
		{
			OpenInNewWindow = request.OpenInNewWindow,
			URL = request.URL,
			IsEnabled = true,
			Order = request.Order,
			Name = request.Name,
			ParentID = request.ParentID,
			Key = request.Key,
		};

		await _context.Menus.AddAsync(menu);
		await _context.SaveChangesAsync();

		await _context.SaveChangesAsync();
		await _menuCachingService.RefreshAll();

		result.SetSuccess(_localizer["RecordAdded"]);
		return result;
	}

	public async Task<ServiceResult<CreateUpdateRequest>> CreateParentMenu(CreateUpdateRequest request)
	{
		var result = new ServiceResult<CreateUpdateRequest> { Data = new CreateUpdateRequest() };

		var exist = await _context.Menus.Where(x => x.Name == request.Name && !x.ParentID.HasValue).AnyAsync();
		if (exist)
		{
			result.SetError(_localizer["RecordAlreadyExist"]);
			return result;
		}

		var menu = new Menu
		{
			OpenInNewWindow = request.OpenInNewWindow,
			URL = request.URL,
			IsEnabled = true,
			Order = request.Order,
			Name = request.Name,
			ParentID = null,
			Key = request.Key,
		};

		await _context.Menus.AddAsync(menu);
		await _context.SaveChangesAsync();

		await _context.SaveChangesAsync();
		await _menuCachingService.RefreshAll();

		result.SetSuccess(_localizer["RecordAdded"]);
		return result;
	}

	public ServiceResult<CreateUpdateRequest> Create(int menuId, int? currentId)
	{
		var result = new ServiceResult<CreateUpdateRequest>();

		result.Data = new CreateUpdateRequest();
		result.Data.DetailsID = menuId;
		if (currentId.HasValue)
		{
			result.Data.ParentID = currentId;
		}
		else
		{
			result.Data.ParentID = menuId;
		}

		return result;
	}

	public ServiceResult<CreateUpdateRequest> CreateParentMenu()
	{
		var result = new ServiceResult<CreateUpdateRequest>();

		result.Data = new CreateUpdateRequest();
		return result;
	}

	public async Task<ServiceResult> Delete(int ID)
	{
		var result = new ServiceResult();

		var menus = await _context.Menus.Where(x => x.ID == ID).ToListAsync();
		if (!menus.Any())
		{
			result.SetError(_localizer["RecordNotFound"]);
			return result;
		}

		var childMenus = await _context.Menus.Where(x => x.ParentID == ID).ToListAsync();

		if (childMenus.Any())
		{
			foreach (var child in childMenus)
			{
				await Delete(child.ID);
			}
		}

		_context.Menus.RemoveRange(menus);
		await _context.SaveChangesAsync();
		await _menuCachingService.RefreshAll();

		result.SetSuccess(_localizer["RecordDeleted"]);
		return result;
	}

	public async Task<ServiceResult<List<GetMenuTreeItem>>> GetMenusTree(int? parent, bool onlyEnableds)
	{
		var result = new ServiceResult<List<GetMenuTreeItem>>();

		var Menu = await (from m in _context.Menus
						  where (parent.HasValue ? m.ParentID == parent : m.ParentID == null)
								&& (!onlyEnableds || m.IsEnabled)
						  orderby m.Order
						  let childCount = _context.Menus.Count(mc => mc.ParentID == m.ID)
						  select new GetMenuTreeItem
						  {
							  Id = m.ID,
							  text = m.Name,
							  icon = m.IsEnabled ? "fa fa-folder text-primary" : "fa fa-folder text-secondary",
							  children = childCount > 0,
							  type = parent.HasValue ? "root" : "child",
							  IsEnabled = m.IsEnabled,
						  }).ToListAsync();

		result.Data = Menu;

		return result;
	}

	public async Task<ServiceResult<CreateUpdateRequest>> Update(int menuId, int currentId)
	{
		var result = new ServiceResult<CreateUpdateRequest>();

		var menu = await _context.Menus.FirstOrDefaultAsync(x => x.ID == currentId);
		if (menu is null)
		{
			result.SetError(_localizer["RecordNotFound"]);
			return result;
		}

		var request = new CreateUpdateRequest
		{
			ParentID = menu.ParentID,
			ID = currentId,
			Name = menu.Name,
			Order = menu.Order,
			IsEnabled = menu.IsEnabled,
			URL = menu.URL,
			OpenInNewWindow = menu.OpenInNewWindow,
			DetailsID = menuId,
			Key = menu.Key,
		};
		result.Data = request;
		return result;
	}

	public async Task<ServiceResult<CreateUpdateRequest>> Update(CreateUpdateRequest request)
	{
		var result = new ServiceResult<CreateUpdateRequest>();

		if (!request.ParentID.HasValue)
		{
			var existing = await _context.Menus
				.Where(x => x.Name == request.Name &&
							!x.ParentID.HasValue &&
							x.ID != request.ID)
				.AnyAsync();
			if (existing)
			{
				result.SetError(_localizer["RecordAlreadyExist"]);
				return result;
			}
		}
		else
		{
			var existing = await _context.Menus
				.Where(x => x.Name.ToUpper().Equals(request.Name.ToUpper()) &&
							x.ParentID == request.ParentID &&
							x.ID != request.ID)
				.AnyAsync();
			if (existing)
			{
				result.SetError(_localizer["RecordAlreadyExist"]);
				return result;
			}
		}

		var menu = await _context.Menus.Where(x => x.ID == request.ID).FirstOrDefaultAsync();

		menu.Name = request.Name;
		menu.Order = request.Order;
		menu.ParentID = request.ParentID;
		menu.IsEnabled = request.IsEnabled;
		menu.URL = request.URL;
		menu.OpenInNewWindow = request.OpenInNewWindow;
		menu.Key = request.Key;

		_context.Menus.Update(menu);
		await _context.SaveChangesAsync();

		await _menuCachingService.RefreshAll();

		result.SetSuccess(_localizer["RecordUpdated"]);
		return result;
	}

	public async Task<ServiceResult<CreateUpdateRequest>> UpdateParent(int ID)
	{
		var result = new ServiceResult<CreateUpdateRequest>();

		var menu = await _context.Menus.FirstOrDefaultAsync(x => x.ID == ID);
		if (menu is null)
		{
			result.SetError(_localizer["RecordNotFound"]);
			return result;
		}

		var request = new CreateUpdateRequest
		{
			ParentID = menu.ParentID,
			ID = ID,
			Name = menu.Name,
			Order = menu.Order,
			IsEnabled = menu.IsEnabled,
			URL = menu.URL,
			OpenInNewWindow = menu.OpenInNewWindow,
			Key = menu.Key,
		};

		result.Data = request;
		return result;
	}

	public async Task<ServiceResult<CreateUpdateRequest>> UpdateParent(CreateUpdateRequest request)
	{
		var result = new ServiceResult<CreateUpdateRequest>();


		var exist = await _context.Menus.Where(x => x.Name == request.Name && !x.ParentID.HasValue && x.ID != request.ID).AnyAsync();
		if (exist)
		{
			result.SetError(_localizer["RecordAlreadyExist"]);
			return result;
		}

		var menu = await _context.Menus.Where(x => x.ID == request.ID).FirstOrDefaultAsync();

		menu.Name = request.Name;
		menu.Order = request.Order;
		menu.ParentID = request.ParentID;
		menu.IsEnabled = request.IsEnabled;
		menu.URL = request.URL;
		menu.OpenInNewWindow = request.OpenInNewWindow;
		menu.Key = request.Key;

		_context.Menus.Update(menu);
		await _context.SaveChangesAsync();

		await _menuCachingService.RefreshAll();

		result.SetSuccess(_localizer["RecordUpdated"]);
		return result;

	}

	public async Task<BaseDataTableResponse<List<ListResponseMenuItem>>> GetAllPaged(BaseDataTableRequest tableRequest)
	{
		var result = new BaseDataTableResponse<List<ListResponseMenuItem>>();

		result.RecordsTotal = _context.Menus.Count();

		var query = (from m in _context.Menus
					 where !m.ParentID.HasValue && m.IsEnabled
					 orderby m.Name
					 select new
					 {
						 ID = m.ID,
						 Name = m.Name
					 }).AsNoTracking().AsQueryable();

		if (!string.IsNullOrEmpty(tableRequest.SearchValue))
		{
			query = query.Where(x =>
				x.Name.ToLower().Contains(tableRequest.SearchValue.ToLower()));
		}

		result.RecordsFiltered = query.Count();

		query = query.SortSkipTake(tableRequest);

		result.Data = await query.Select(x => new ListResponseMenuItem
		{
			ID = x.ID,
			Name = x.Name,
		}).ToListAsync();

		return result;
	}

	public async Task<ServiceResult<MenuDetailsRequest>> Details(int id)
	{
		var result = new ServiceResult<MenuDetailsRequest>();

		var request = await (from m in _context.Menus
							 where id == m.ID
							 select new MenuDetailsRequest
							 {
								 Name = m.Name,
								 ID = m.ID
							 }).FirstOrDefaultAsync();

		result.Data = request;

		return result;
	}
}