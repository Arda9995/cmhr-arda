using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.NewsIntegrationRequest;
using Quark.CMS.Business.Models.Responses.NewsIntegrationResponses;
using Quark.Data;
using Quark.Data.Models.News;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class NewsIntegrationService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), INewsIntegrationService
{
    public async Task<BaseDataTableResponse<List<ListNewsIntegrationItemResponse>>> GetAllPagedAsync(
        BaseDataTableRequest tableRequest)
    {
        var response = new BaseDataTableResponse<List<ListNewsIntegrationItemResponse>>();

        var query = _context.PrintingNews.AsNoTracking().AsQueryable();

        response.RecordsTotal = await query.CountAsync();

        if (!tableRequest.SearchValue.IsNullOrEmtpy())
        {
            query = query.Where(x => x.Title.Contains(tableRequest.SearchValue) ||
                                     x.Content.Contains(tableRequest.SearchValue));
        }

        response.RecordsFiltered = await query.CountAsync();

        query = query.SortSkipTake(tableRequest);

        var data = await query
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new ListNewsIntegrationItemResponse
            {
                ID = x.ID.ToString(),
                NewsID = x.NewsID != null ? x.NewsID.ToString() : string.Empty,
                Title = x.Title,
            })
            .ToListAsync();

        response.Data = data;
        return response;
    }

    public async Task<ServiceResult<CreateNewsIntegrationRequest>> CreateAsync(long? newsId)
    {
        var result = new ServiceResult<CreateNewsIntegrationRequest>();
        var data = new CreateNewsIntegrationRequest();
        int? selectedCategoryId = null;
        
        if (newsId.HasValue)
        {
            var news = await _context.News.Where(x=>x.ID == newsId).FirstOrDefaultAsync();

            if (news == null)
            {
                result.SetError("Haber bulunamadı.");
                return result;
            }

            data.NewsID = news.ID;
            data.Title = news.Title;
            data.Content = news.Content;
            data.CategoryID = news.CategoryID;

            selectedCategoryId = news.CategoryID;
        }
        
        data.Categories = await _context.Categories
            .AsNoTracking()
            .Where(x => x.IsEnabled)
            .Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.ID.ToString(),
                Selected = selectedCategoryId.HasValue && x.ID == selectedCategoryId.Value
            })
            .ToListAsync();

        result.Data = data;
        return result;
    }

    public async Task<ServiceResult<CreateNewsIntegrationRequest>> CreateAsync(CreateNewsIntegrationRequest request)
    {
        var result = new ServiceResult<CreateNewsIntegrationRequest>();

        #region Validation

        if (request.Title.IsNullOrEmtpy() || request.Content.IsNullOrEmtpy())
        {
            result.SetError("Başlık ve içerik alanları zorunludur.");
            return result;
        }

        #endregion

        var newsPrint = new PrintingNews()
        {
            NewsID = request.NewsID,
            CategoryID = request.CategoryID,
            Title = request.Title,
            Content = request.Content,
            CreatedBy = CurrentUserID,
            CreatedDate = DateTime.Now
        };

        await _context.PrintingNews.AddAsync(newsPrint);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<ServiceResult<UpdateNewsIntegrationRequest>> UpdateAsync(long newsIntegrationId)
    {
        var result = new ServiceResult<UpdateNewsIntegrationRequest>();

        var newsIntegration = await _context.PrintingNews.Where(x => x.ID == newsIntegrationId).FirstOrDefaultAsync();
        if (newsIntegration == null)
        {
            result.SetError("Yazılı haber entegrasyonu bulunamadı.");
            return result;
        }

        var data = new UpdateNewsIntegrationRequest
        {
            ID = newsIntegration.ID,
            CategoryID = newsIntegration.CategoryID ?? 0,
            Title = newsIntegration.Title,
            Content = newsIntegration.Content
        };

        var categories = await _context.Categories.Where(x => x.IsEnabled).Select(x => new SelectListItem()
        {
            Text = x.Name,
            Value = x.ID.ToString(),
            Selected = x.ID == data.CategoryID
        }).ToListAsync();

        data.Categories = categories;
        result.Data = data;
        return result;
    }

    public async Task<ServiceResult<UpdateNewsIntegrationRequest>> UpdateAsync(UpdateNewsIntegrationRequest request)
    {
        var result = new ServiceResult<UpdateNewsIntegrationRequest>();

        var newsIntegration = await _context.PrintingNews.Where(x => x.ID == request.ID).FirstOrDefaultAsync();
        if (newsIntegration == null)
        {
            result.SetError("Yazılı haber entegrasyonu bulunamadı.");
            return result;
        }

        newsIntegration.CategoryID = request.CategoryID;
        newsIntegration.Title = request.Title;
        newsIntegration.Content = request.Content;

        _context.PrintingNews.Update(newsIntegration);
        await _context.SaveChangesAsync();

        return result;
    }
}