using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CaricaturistRequests;
using Quark.CMS.Business.Models.Responses.CaricaturistResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class CaricaturistService : BaseService, ICaricaturistService
{
    private readonly StatisticsDataContext _statisticsContext;

    public CaricaturistService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, StatisticsDataContext statisticsContext) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _statisticsContext = statisticsContext;
    }

    public async Task<BaseDataTableResponse<List<ListResponseCaricaturistItem>>> GetAllPaged(BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<ListResponseCaricaturistItem>>();

        result.RecordsTotal = _context.Caricaturists.Count();

        var query = (from c in _context.Caricaturists
                     orderby c.ID
                     select new
                     {
                         ID = c.ID,
                         Name = c.Name,
                         Headline = c.Headline
                     }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseCaricaturistItem
        {
            ID = x.ID,
            Name = x.Name,
            Headline = x.Headline
        }).ToListAsync();

        return result;
    }

    public ServiceResult<CreateCaricaturistRequest> Create()
    {
        var result = new ServiceResult<CreateCaricaturistRequest>();

        return result;
    }

    public async Task<ServiceResult<CreateCaricaturistRequest>> Create(CreateCaricaturistRequest request)
    {
        var result = new ServiceResult<CreateCaricaturistRequest>();

        var isExist = await _context.Caricaturists.AnyAsync(x => x.Name == request.Name);
        if (isExist)
        {
            result.SetError(_localizer["RecordAlreadyExists"]);
            return result;
        }

        var caricaturist = new Caricaturist
        {
            Name = request.Name,
            Headline = request.Headline
        };

        await _context.Caricaturists.AddAsync(caricaturist);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<ServiceResult<UpdateCaricaturistRequest>> Update(int id)
    {
        var result = new ServiceResult<UpdateCaricaturistRequest>();

        var caricaturist = await _context.Caricaturists.FirstOrDefaultAsync(x => x.ID == id);
        if (caricaturist == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var request = new UpdateCaricaturistRequest
        {
            ID = id,
            Name = caricaturist.Name,
            Headline = caricaturist.Headline
        };

        result.Data = request;
        return result;
    }

    public async Task<ServiceResult<UpdateCaricaturistRequest>> Update(UpdateCaricaturistRequest request)
    {
        var result = new ServiceResult<UpdateCaricaturistRequest>();

        var isExist = await _context.Caricaturists.AnyAsync(x => x.Name == request.Name && x.Headline == request.Headline);
        if (isExist)
        {
            result.SetError(_localizer["RecordAlreadyExists"]);
            return result;
        }

        var caricaturist = await _context.Caricaturists.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (caricaturist == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        caricaturist.Name = request.Name;
        caricaturist.Headline = request.Headline;

        _context.Caricaturists.Update(caricaturist);
        await _context.SaveChangesAsync();

        result.Data = new UpdateCaricaturistRequest { Headline = request.Headline, ID = caricaturist.ID, Name = request.Name };
        return result;
    }

    public async Task<ServiceResult<CaricaturistDetailsResponse>> Details(int id)
    {
        var result = new ServiceResult<CaricaturistDetailsResponse>();

        var caricaturist = await _context.Caricaturists.FirstOrDefaultAsync(x => x.ID == id);
        if (caricaturist == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var response = new CaricaturistDetailsResponse
        {
            ID = caricaturist.ID,
            Name = caricaturist.Name,
            Headline = caricaturist.Headline
        };

        result.Data = response;
        return result;
    }

    public async Task<BaseDataTableResponse<List<DetailsCaricaturistNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetCaricaturistFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<DetailsCaricaturistNewsResponseItem>>();

        var query = (from n in _statisticsContext.News
                     join s in _statisticsContext.NewsStatus on n.StatusID equals s.ID
                     join c in _statisticsContext.Categories on n.CategoryID equals c.ID
                     join p in _statisticsContext.NewsPhotos on n.ID equals p.NewsID
                     join t in _statisticsContext.NewsTypes on n.NewsTypeID equals t.ID
                     join v in _statisticsContext.NewsViewsDailies on n.ID equals v.NewsID
                     join ca in _statisticsContext.Caricaturists on n.AuthorID equals ca.ID
                     where p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && ca.ID == request.ID
                     orderby n.ID descending
                     select new
                     {
                         ID = n.ID,
                         Title = n.Title,
                         Category = c.Name,
                         Type = t.Name,
                         CreatedDate = n.CreatedDate,
                         UpdatedDate = n.UpdateDate,
                         Status = s.Name,
                         ViewCount = n.ViewCount,
                         PhotoUrl = p.PhotoUrl,
                         PublishedDate = n.PublishDate,
                         Tags = n.Tags,
                         IsLockedAuthorized = (n.IsLocked && !(n.LockedBy == CurrentUserID)),
                         StatusId = n.StatusID
                     }).AsNoTracking().AsQueryable();

        result.RecordsTotal = await query.CountAsync();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Title.ToUpper().Contains(tableRequest.SearchValue.ToUpper()));
        }

        result.RecordsFiltered = await query.CountAsync();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(f => new DetailsCaricaturistNewsResponseItem
        {
            ID = f.ID,
            Category = f.Category,
            CreatedDate = f.CreatedDate.ToFormattedDateTime(),
            UpdatedDate = f.UpdatedDate.ToFormattedDateTime(),
            Status = f.Status,
            Title = f.Title,
            PhotoUrl = f.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                _appSettingsKeys.CDN_MediaGalleryFolder),
            ViewCount = f.ViewCount.ToString("N0"),
            PublishedDate = f.PublishedDate.ToFormattedDateTime().ToEmptyStringIndicator(),
            Type = f.Type,
            IsLockedAuthorized = f.IsLockedAuthorized,
            Tags = f.Tags,
            RelatedNewsCount = _statisticsContext.RelatedNews.Count(x => x.NewsID == f.ID),
            TitleTooltip = f.Title,
            StatusId = f.StatusId
        }).ToListAsync();

        #region Impressions için dummy data

        foreach (var item in result.Data)
        {
            if (item.Status == "Yayında")
            {
                // TODO : Viewlar dinamik bir şekilde çekilmeye başlandığında alttaki dummy kodu silinip bu kod açılacak
                var impressionsList = await _statisticsContext.NewsViewsMinutelies.Where(x =>
                        x.NewsID == item.ID && DateTime.Now.AddMinutes(-15) <= x.ViewsDate)
                    .ToListAsync();

                if (impressionsList != null && impressionsList.Count > 0)
                {
                    var endTime = DateTime.Now;
                    var startTime = impressionsList.First().ViewsDate;

                    var allMinutes = Enumerable.Range(0, (int)(endTime - startTime).TotalMinutes + 1)
                        .Select(offset => startTime.AddMinutes(offset))
                        .ToList();

                    var completeData = allMinutes.Select(time => new
                    {
                        ViewsDate = time,
                        ViewCount = impressionsList.FirstOrDefault(v => v.ViewsDate == time)?.ViewCount ?? 0
                    }).ToList();

                    item.Impressions = completeData.Select(x => x.ViewCount).ToList();
                    item.ImpressionDates = completeData.Select(x => x.ViewsDate).ToList();
                    item.IsImpressionIncreasing = item.Impressions.First() < item.Impressions.Last();

                }
                else
                {
                    item.Impressions = new List<int>();
                    item.ImpressionDates = new List<DateTime>();
                }
            }

            if (!item.Tags.IsNullOrEmtpy())
            {
                item.TagsCount = item.Tags.Split(',').Length;
            }

            if (item.Title.Length > 100)
            {
                item.Title = item.Title.Substring(0, 100) + "...";
            }
        }

        #endregion

        return result;
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var result = new ServiceResult();

        var caricaturist = await _context.Caricaturists.FirstOrDefaultAsync(x => x.ID == id);
        if (caricaturist == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        _context.Caricaturists.Remove(caricaturist);
        await _context.SaveChangesAsync();

        return result;
    }
}