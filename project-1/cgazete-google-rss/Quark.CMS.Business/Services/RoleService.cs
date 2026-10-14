using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.RoleRequests;
using Quark.CMS.Business.Models.Responses.RoleResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AvvaMobile.Core.Caching;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;
using Quark.CMS.Business.Models.Responses.CommonResponses;
using AvvaMobile.Core.ExcelExport;

namespace Quark.CMS.Business.Services;

public class RoleService : BaseService, IRoleService
{
    public RoleService(QuarkDataContext context, IHttpContextAccessor httpContext, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public ServiceResult<GetFiltersRequest> GetAllPagedFiltersAsync()
    {
        var result = new ServiceResult<GetFiltersRequest>();

        var filters = new GetFiltersRequest { IsEnabled = true };

        result.Data = filters;
        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, GetFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<ListResponseItem>>();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        result.RecordsTotal = _context.Roles.Count();

        var query = (from rl in _context.Roles
            where rl.IsEnabled == request.IsEnabled
            select new
            {
                ID = rl.ID,
                Name = rl.Name,
                Description = rl.Description,
                ActiveUserCount = _context.Users.Where(x => x.RoleID == rl.ID && x.IsEnabled == true).Count(),
                PassiveUserCount = _context.Users.Where(x => x.RoleID == rl.ID && x.IsEnabled == false).Count(),
                IsEnabled = rl.IsEnabled,
                IsEnabledText = rl.IsEnabled ? enabledText : disabledText,
            }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x => x.Name.Contains(tableRequest.SearchValue) || x.Description.Contains(tableRequest.SearchValue));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Name = x.Name,
            Description = x.Description,
            ActiveUserCount = x.ActiveUserCount,
            PassiveUserCount = x.PassiveUserCount,
            IsEnabled = x.IsEnabled,
            IsEnabledText = x.IsEnabledText,
        }).ToListAsync();

        return result;
    }
    public async Task<ServiceResult<ExcelExportResponse>> GetAllPagedAsync(GetFiltersRequest request)
    {
        var result = new ServiceResult<ExcelExportResponse>();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        var query = (from rl in _context.Roles
            where rl.IsEnabled == request.IsEnabled
            select new
            {
                ID = rl.ID,
                Name = rl.Name,
                Description = rl.Description,
                ActiveUserCount = _context.Users.Where(x => x.RoleID == rl.ID && x.IsEnabled == true).Count(),
                PassiveUserCount = _context.Users.Where(x => x.RoleID == rl.ID && x.IsEnabled == false).Count(),
                IsEnabled = rl.IsEnabled,
                IsEnabledText = rl.IsEnabled ? enabledText : disabledText,
            }).AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(x => x.Name.Contains(request.SearchValue) || x.Description.Contains(request.SearchValue));
        }

            

        var list = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Name = x.Name,
            Description = x.Description,
            ActiveUserCount = x.ActiveUserCount,
            PassiveUserCount = x.PassiveUserCount,
            IsEnabled = x.IsEnabled,
            IsEnabledText = x.IsEnabledText,
        }).ToListAsync();

        result.Data = new ExcelExportResponse
        {
            FileBytes = list.ExportToExcel(),
            FileName = "Roller.xlsx",
        };
        return result;
    }
    public async Task<ServiceResult<CreateUpdateRequest>> CreateAsync()
    {
        var result = new ServiceResult<CreateUpdateRequest>();

        var createRoleRequest = new CreateUpdateRequest
        {
            SecurityClaims = await (from sc in _context.SecurityClaims
                select new SecurityClaimItem
                {
                    Key = sc.Key,
                    Description = sc.Description,
                    IsAllowed = false
                }).ToListAsync()
        };

        result.Data = createRoleRequest;
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> CreateAsync(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest>();

        var existedRole = await _context.Roles.Where(x => x.Name == request.Name.Trim()).FirstOrDefaultAsync();

        if (existedRole != null)
        {
            var createRoleRequest = new CreateUpdateRequest
            {
                SecurityClaims = await (from sc in _context.SecurityClaims
                    select new SecurityClaimItem
                    {
                        Key = sc.Key,
                        Description = sc.Description,
                        IsAllowed = false
                    }).ToListAsync()
            };

            result.Data = createRoleRequest;
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        var role = new Role
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            IsEnabled = request.IsEnabled
        };
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        foreach (var item in request.SecurityClaims)
        {
            if (item.IsAllowed)
            {
                var roleClaim = new RoleClaim
                {
                    RoleID = role.ID,
                    Key = item.Key,
                };
                await _context.RoleClaims.AddAsync(roleClaim);
            }
        }

        await _context.SaveChangesAsync();

        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(int id)
    {
        var result = new ServiceResult<CreateUpdateRequest>();

        var role = await (from rl in _context.Roles
            where rl.ID == id
            select new CreateUpdateRequest
            {
                ID = rl.ID,
                Name = rl.Name.Trim(),
                Description = rl.Description.Trim(),
                IsEnabled = rl.IsEnabled
            }).FirstOrDefaultAsync();
        if (role is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        role.SecurityClaims = await (from sc in _context.SecurityClaims
            select new SecurityClaimItem
            {
                Key = sc.Key,
                Description = sc.Description,
                IsAllowed = _context.RoleClaims.Any(x => x.RoleID == role.ID && x.Key == sc.Key)
            }).ToListAsync();
        result.Data = role;
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest>();

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (role is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        role.Name = request.Name;
        role.Description = request.Description;
        role.IsEnabled = request.IsEnabled;
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();

        foreach (var item in request.SecurityClaims)
        {
            var roleClaim = await _context.RoleClaims.FirstOrDefaultAsync(x => x.RoleID == request.ID && x.Key == item.Key);
            if (roleClaim is null && item.IsAllowed)
            {
                await _context.RoleClaims.AddAsync(new RoleClaim { RoleID = request.ID, Key = item.Key });
            }
            else if (roleClaim != null && !item.IsAllowed)
            {
                _context.RoleClaims.Remove(roleClaim);
            }
        }

        await _context.SaveChangesAsync();

        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var result = new ServiceResult();

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.ID == id);
        if (role is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        var roleClaims = await _context.RoleClaims.Where(x => x.RoleID == id).ToListAsync();
        if (roleClaims.Count > 0)
        {
            _context.RoleClaims.RemoveRange(roleClaims);
            await _context.SaveChangesAsync();
        }

        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }
}