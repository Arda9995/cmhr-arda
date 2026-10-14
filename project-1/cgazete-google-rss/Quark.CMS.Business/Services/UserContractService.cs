using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.UserContractRequests;
using Quark.CMS.Business.Models.Responses.UserContractResponses;
using Quark.Data;
using Quark.Data.Models.UserContracts;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class UserContractService : BaseService, IUserContractService
{
    public UserContractService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<BaseDataTableResponse<List<ListResponseUserContractItem>>> GetAllPaged(BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<ListResponseUserContractItem>>();

        result.RecordsTotal = await _context.UserContracts.CountAsync();

        var query = (from uc in _context.UserContracts
            select new
            {
                ID = uc.ID,
                Title = uc.Title,
                Content = uc.Content
            }).AsNoTracking().AsQueryable();
        
        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Title.ToUpper().Contains(tableRequest.SearchValue.ToUpper()));
        }
        
        result.RecordsFiltered = await query.CountAsync();
        
        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(f => new ListResponseUserContractItem
        {
            ID = f.ID,
            Title = f.Title,
            Content = f.Content
        }).ToListAsync();
        
        return result;
    }

    public ServiceResult<CreateUserContractRequest> Create()
    {
        var result = new ServiceResult<CreateUserContractRequest> { Data = new CreateUserContractRequest()};
        
        return result;
    }

    public async Task<ServiceResult> Create(CreateUserContractRequest request)
    {
        var result = new ServiceResult();

        var isExist = await _context.UserContracts.AnyAsync(x => x.Title == request.Title);

        if (isExist)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        var userContract = new UserContract
        {
            Title = request.Title,
            Content = request.Content
        };

        await _context.UserContracts.AddAsync(userContract);
        await _context.SaveChangesAsync();
        
        result.Message = _localizer["RecordAdded"];
        return result;
    }

    public async Task<ServiceResult<UpdateUserContractRequest>> Update(int id)
    {
        var result = new ServiceResult<UpdateUserContractRequest>();

        var userContract = await _context.UserContracts.FirstOrDefaultAsync(x => x.ID == id);

        if (userContract is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var request = new UpdateUserContractRequest
        {
            ID = userContract.ID,
            Title = userContract.Title,
            Content = userContract.Content
        };

        result.Data = request;
        return result;
    }

    public async Task<ServiceResult> Update(UpdateUserContractRequest request)
    {
        var result = new ServiceResult();

        var userContract = await _context.UserContracts.FirstOrDefaultAsync(x => x.ID == request.ID);

        if (userContract is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var isExist = await _context.UserContracts.AnyAsync(x => x.ID != request.ID && x.Title == request.Title);
        
        if (isExist)
        {
            result.SetError(_localizer["RecordAlreadyExists"]);
            return result;
        }

        userContract.Title = request.Title;
        userContract.Content = request.Content;

        _context.UserContracts.Update(userContract);
        await _context.SaveChangesAsync();
        
        result.Message = _localizer["RecordUpdated"];
        return result;
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var result = new ServiceResult();

        var userContract = await _context.UserContracts.FirstOrDefaultAsync(x => x.ID == id);

        if (userContract is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        _context.UserContracts.Remove(userContract);
        await _context.SaveChangesAsync();

        result.Message = _localizer["RecordDeleted"];
        return result;
    }
}