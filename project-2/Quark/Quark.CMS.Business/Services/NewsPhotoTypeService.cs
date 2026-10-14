using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.ExcelExport;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CommonRequests;
using Quark.CMS.Business.Models.Requests.NewsPhotoTypeRequests;
using Quark.CMS.Business.Models.Responses.CommonResponses;
using Quark.CMS.Business.Models.Responses.NewsPhotoTypeResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using System.Linq.Dynamic.Core;

namespace Quark.CMS.Business.Services;

public class NewsPhotoTypeService : BaseService, INewsPhotoTypeService
{
    private readonly INewsPhotoTypeCachingService _newsPhotoTypeCachingService;
    public NewsPhotoTypeService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, INewsPhotoTypeCachingService newsPhotoTypeCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _newsPhotoTypeCachingService = newsPhotoTypeCachingService;
    }

    public ServiceResult<CreateUpdateRequest> Create()
    {
        var result = new ServiceResult<CreateUpdateRequest>();
        var createNewsPhotoType = new CreateUpdateRequest();

        result.Data = createNewsPhotoType;
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> CreateAsync(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest> { Data = request };

        var isNewsPhotoTypeExist = await _context.NewsPhotoTypes.Where(x => x.Name == request.Name.Trim()).AnyAsync();

        if (isNewsPhotoTypeExist)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        var newsPhotoType = new NewsPhotoType
        {
            ID = request.ID,
            Key = request.Key,
            Name = request.Name,
            Description = request.Description,
            Width = request.Width,
            Height = request.Height,
            IsRequired = request.IsRequired,
            Order = request.Order,
            IsEnabled = request.IsEnabled,
        };

        await _context.NewsPhotoTypes.AddAsync(newsPhotoType);
        await _context.SaveChangesAsync();

        await _newsPhotoTypeCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var result = new ServiceResult();

        var newsPhotoType = await _context.NewsPhotoTypes.FirstOrDefaultAsync(x => x.ID == id);

        if (newsPhotoType == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
        _context.NewsPhotoTypes.Remove(newsPhotoType);
        await _context.SaveChangesAsync();

        await _newsPhotoTypeCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }

    public async Task<ServiceResult<ExcelExportResponse>> DownloadExcelExportAsync(ExcelExportRequest request)
    {
        var result = new ServiceResult<ExcelExportResponse>();

        var query = (from t in _context.NewsPhotoTypes
                     orderby t.Name
                     select new
                     {
                         ID = t.ID,
                         Key = t.Key,
                         Name = t.Name,
                         Description = t.Description,
                         Width = t.Width,
                         Height = t.Height,
                         IsRequired = t.IsRequired,
                         Order = t.Order,
                         IsEnabled = t.IsEnabled,
                     }).AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(request.SearchValue.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.SortColumn) || !string.IsNullOrEmpty(request.SortDirection))
        {
            query = query.OrderBy(request.SortColumn + " " + request.SortDirection);
        }

        var list = await query.Select(x => new ListResponseExcelItem
        {
            ID = x.ID,
            Key = x.Key,
            Name = x.Name,
            Width = x.Width,
            Height = x.Height,
            Order = x.Order,
            IsEnabledText = x.IsEnabled ? "Aktif" : "Pasif",
            IsRequiredText = x.IsRequired ? "Evet" : "Hayır",
        }).ToListAsync();

        result.Data = new ExcelExportResponse
        {
            FileBytes = list.ExportToExcel(),
            FileName = "Haber_Fotograf_Ayarlari.xlsx"
        };

        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<ListResponseItem>>();

        result.RecordsTotal = _context.NewsPhotoTypes.Count();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        var query = (from t in _context.NewsPhotoTypes
                     orderby t.Order
                     select new
                     {
                         ID = t.ID,
                         Key = t.Key,
                         Name = t.Name,
                         Description = t.Description,
                         Width = t.Width,
                         Height = t.Height,
                         IsRequired = t.IsRequired,
                         Order = t.Order,
                         IsEnabled = t.IsEnabled,
                         IsEnabledText = t.IsEnabled ? enabledText : disabledText,
                         IsRequiredText = t.IsRequired ? "Evet" : "Hayır",
                     }).AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Key = x.Key,
            Name = x.Name,
            Width = x.Width,
            Height = x.Height,
            IsRequired = x.IsRequired,
            IsEnabled = x.IsEnabled,
            IsEnabledText = x.IsEnabledText,
            IsRequiredText = x.IsRequiredText,
            Order = x.Order,
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(int id)
    {
        var result = new ServiceResult<CreateUpdateRequest>();

        var newsPhotoType = await (from t in _context.NewsPhotoTypes
                                   where t.ID == id
                                   select new CreateUpdateRequest
                                   {
                                       ID = t.ID,
                                       Key = t.Key,
                                       Name = t.Name,
                                       Description = t.Description,
                                       Width = t.Width,
                                       Height = t.Height,
                                       IsRequired = t.IsRequired,
                                       Order = t.Order,
                                       IsEnabled = t.IsEnabled,
                                   }).FirstOrDefaultAsync();

        if (newsPhotoType is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        result.Data = newsPhotoType;
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest> { Data = request };

        var newsPhotoTypes = await _context.NewsPhotoTypes.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (newsPhotoTypes is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var isNewsPhotoTypesExist = await _context.NewsPhotoTypes.Where(x => x.Name == request.Name.Trim() && x.ID != request.ID).AnyAsync();

        if (isNewsPhotoTypesExist)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        newsPhotoTypes.Name = request.Name;
        newsPhotoTypes.Key = request.Key;
        newsPhotoTypes.Description = request.Description;
        newsPhotoTypes.Width = request.Width;
        newsPhotoTypes.Height = request.Height;
        newsPhotoTypes.Order = request.Order;
        newsPhotoTypes.IsRequired = request.IsRequired;
        newsPhotoTypes.IsEnabled = request.IsEnabled;


        _context.NewsPhotoTypes.Update(newsPhotoTypes);
        await _context.SaveChangesAsync();

        await _newsPhotoTypeCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }
}