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
using Quark.CMS.Business.Models.Requests.PromptRequests;
using Quark.CMS.Business.Models.Responses.PromptResponses;
using Quark.Data;
using Quark.Data.Models.AI;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class PromptService : BaseService, IPromptService
{
    private readonly IPromptCachingService _promptCachingService;

    public PromptService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IPromptCachingService promptCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _promptCachingService = promptCachingService;
    }

    public async Task<ServiceResult<GetPromptFiltersRequest>> GetAllPagedFilters()
    {
        var result = new ServiceResult<GetPromptFiltersRequest>();
        result.Data = new GetPromptFiltersRequest();

        var promptTypeList = await _promptCachingService.GetPromptTypesAsSelectListItems();
        result.Data.PromptTypes = promptTypeList.Data;
        result.Data.PromptTypes.AddALLOption();

        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponsePromptItem>>> GetAllPaged(BaseDataTableRequest tableRequest, GetPromptFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<ListResponsePromptItem>>();

        var query = (from p in _context.Prompts
                     join pt in _context.PromptTypes on p.PromptTypeID equals pt.ID
                     where request.SelectedPromptTypeID.IsALLSelected() || request.SelectedPromptTypeID == pt.ID
                     select new
                     {
                         ID = p.ID,
                         Name = p.Name,
                         PromptType = pt.Name
                     }).AsNoTracking().AsQueryable();

        result.RecordsTotal = await query.CountAsync();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Name.ToUpper().Contains(tableRequest.SearchValue.ToUpper()) ||
                x.PromptType.ToUpper().Contains(tableRequest.SearchValue.ToUpper()));
        }

        result.RecordsFiltered = await query.CountAsync();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponsePromptItem
        {
            ID = x.ID,
            Name = x.Name,
            PromptType = x.PromptType
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<CreatePromptRequest>> Create()
    {
        var result = new ServiceResult<CreatePromptRequest>();
        result.Data = new CreatePromptRequest();

        var promptTypeList = await _promptCachingService.GetPromptTypesAsSelectListItems();
        result.Data.PromptTypes = promptTypeList.Data;
        result.Data.PromptTypes.AddSELECTOption();

        result.Data.RequestTypes = await _context.RequestTypes.Select(x => new SelectListItem
        {
            Value = x.ID.ToString(),
            Text = x.Name
        }).ToListAsync();
        result.Data.RequestTypes.AddSELECTOption();

        return result;
    }

    public async Task<ServiceResult<CreatePromptRequest>> Create(CreatePromptRequest request)
    {
        var result = new ServiceResult<CreatePromptRequest>();
        result.Data = new CreatePromptRequest();

        var isExist = await _context.Prompts.AnyAsync(x => x.Name == request.Name);

        if (isExist)
        {
            var promptTypeList = await _promptCachingService.GetPromptTypesAsSelectListItems();

            result.Data.PromptTypes = promptTypeList.Data;
            result.Data.PromptTypes.AddSELECTOption();

            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        var isMessageExist = await _context.Prompts.Where(x =>
            x.Message == request.Message && x.PromptTypeID == request.PromptTypeID).AnyAsync();

        if (isMessageExist)
        {
            var promptTypeList = await _promptCachingService.GetPromptTypesAsSelectListItems();

            result.Data.PromptTypes = promptTypeList.Data;
            result.Data.PromptTypes.AddSELECTOption();

            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        var prompt = new Prompt
        {
            Name = request.Name.Trim(),
            PromptTypeID = request.PromptTypeID,
            Message = request.Message,
            RequestTypeID = request.RequestTypeID
        };

        await _context.Prompts.AddAsync(prompt);
        await _context.SaveChangesAsync();

        await _promptCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult<UpdatePromptRequest>> Update(int id)
    {
        var result = new ServiceResult<UpdatePromptRequest>();

        var prompt = await (from p in _context.Prompts
                            where p.ID == id
                            select new UpdatePromptRequest
                            {
                                ID = p.ID,
                                Name = p.Name,
                                PromptTypeID = p.PromptTypeID,
                                Message = p.Message,
                                RequestTypeID = p.RequestTypeID
                            }).FirstOrDefaultAsync();

        if (prompt is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var promptTypeList = await _promptCachingService.GetPromptTypesAsSelectListItems();
        prompt.PromptTypes = promptTypeList.Data;
        prompt.PromptTypes.AddSELECTOption();

        var requestTypeList = await _context.RequestTypes.Select(x => new SelectListItem
        {
            Value = x.ID.ToString(),
            Text = x.Name
        }).ToListAsync();
        prompt.RequestTypes = requestTypeList;
        prompt.RequestTypes.AddSELECTOption();

        result.Data = prompt;

        return result;
    }

    public async Task<ServiceResult<UpdatePromptRequest>> Update(UpdatePromptRequest request)
    {
        var result = new ServiceResult<UpdatePromptRequest>();
        result.Data = new UpdatePromptRequest();

        var isNameExist = await _context.Prompts.Where(x => x.Name == request.Name.Trim() && x.ID != request.ID).AnyAsync();

        if (isNameExist)
        {
            var promptTypeList = await _promptCachingService.GetPromptTypesAsSelectListItems();

            result.Data.PromptTypes = promptTypeList.Data;
            result.Data.PromptTypes.AddSELECTOption();

            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        var isMessageExist = await _context.Prompts.Where(x =>
            x.Message == request.Message && x.ID != request.ID && x.PromptTypeID == request.PromptTypeID).AnyAsync();

        if (isMessageExist)
        {
            var promptTypeList = await _promptCachingService.GetPromptTypesAsSelectListItems();

            result.Data.PromptTypes = promptTypeList.Data;
            result.Data.PromptTypes.AddSELECTOption();

            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        var prompt = await _context.Prompts.FirstOrDefaultAsync(x => x.ID == request.ID);

        if (prompt is null)
        {
            var promptTypeList = await _promptCachingService.GetPromptTypesAsSelectListItems();

            result.Data.PromptTypes = promptTypeList.Data;
            result.Data.PromptTypes.AddSELECTOption();

            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        prompt.Name = request.Name.Trim();
        prompt.Message = request.Message;
        prompt.PromptTypeID = request.PromptTypeID;
        prompt.RequestTypeID = request.RequestTypeID;

        _context.Prompts.Update(prompt);
        await _context.SaveChangesAsync();

        await _promptCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var result = new ServiceResult();

        var prompt = await _context.Prompts.FirstOrDefaultAsync(x => x.ID == id);
        if (prompt is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        _context.Prompts.Remove(prompt);
        await _context.SaveChangesAsync();

        await _promptCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }
}