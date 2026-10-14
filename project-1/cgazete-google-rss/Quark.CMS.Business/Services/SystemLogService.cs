using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.SystemLogRequests;
using Quark.CMS.Business.Models.Responses.SystemLogResponses;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class SystemLogService(
    QuarkDataContext context,
    IHttpContextAccessor httpContext,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    ISystemLogCachingService _systemLogCachingService)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), ISystemLogService
{
    public async Task<ServiceResult<ListFiltersRequest>>  GetAllPagedFiltersAsync()
    {
        var result = new ServiceResult<ListFiltersRequest> { Data = new ListFiltersRequest() };

        var types = await _systemLogCachingService.GetLogTypesAsSelectListItems();
        result.Data.Types = types.Data;
        result.Data.Types.AddALLOption();
        
        return result;
    }
    public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, ListFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<ListResponseItem>>();

        result.RecordsTotal = _context.SystemLogs.Count();

        string successText = _localizer["successText"];
        string failText = _localizer["failText"];

        var query = (from l in _context.SystemLogs
            join u in _context.Users on l.UserID equals u.ID into userJoin
            from uj in userJoin.DefaultIfEmpty()
            join t in _context.LogTypes on l.LogTypeID equals t.ID
            where request.SelectedTypeID.IsALLSelected() || request.SelectedTypeID == t.ID
                     orderby l.ID descending
                     select new
                     {
                         ID = l.ID,
                         UserID = l.UserID,
                         FirstName = uj.Firstname,
                         LastName = uj.Lastname,
                         UserName = uj.FullName,
                         Credential = l.Credential,
                         IpAddress = l.IpAddress,
                         CreatedDate = l.CreatedDate,
                         Message = l.Message,
                         IsSuccess = l.IsSuccess,
                         IsSuccessText = l.IsSuccess ? successText : failText,
                         LogType = t.Name,
                         Url = t.Url,
                         ObjectID = l.ObjectID
                     }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.FirstName.ToUpper().Contains(tableRequest.SearchValue.ToUpper()) ||
                x.LastName.ToUpper().Contains(tableRequest.SearchValue.ToUpper()) ||
                x.Credential.ToUpper().Contains(tableRequest.SearchValue.ToUpper()));
        }

        result.RecordsFiltered = await query.CountAsync();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            UserID = x.UserID,
            UserName = x.UserName.ToEmptyStringIndicator(),
            Credential = x.Credential,
            IpAddress = x.IpAddress,
            CreatedDate = x.CreatedDate.ToFormattedDateTime(),
            Message = x.Message,
            IsSuccess = x.IsSuccess,
            IsSuccessText = x.IsSuccessText,
            LogType = x.LogType,
            Url = x.Url + x.ObjectID.ToString(),
            ObjectID = x.ObjectID
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<DetailsResponse>> Details(int id)
    {
        var result = new ServiceResult<DetailsResponse>();

        var log = await (from l in _context.SystemLogs
            join u in _context.Users on l.UserID equals u.ID into userJoin
            from uj in userJoin.DefaultIfEmpty()
            join t in _context.LogTypes on l.LogTypeID equals t.ID
            where l.ID == id
                select new DetailsResponse
            {
                ID = l.ID,
                UserName = uj.FullName,
                CreatedDate = l.CreatedDate.ToFormattedDateTime(),
                Credentials = l.Credential,
                IpAdress = l.IpAddress,
                Url = t.Url + l.ObjectID,
                Message = l.Message,
                LogType = t.Name,
                LogTypeID = l.LogTypeID
            }).FirstOrDefaultAsync();

        if (log is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
        
        result.Data = log;
        
        return result;
    }
}