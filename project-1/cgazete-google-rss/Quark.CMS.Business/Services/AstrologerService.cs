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
using Quark.CMS.Business.Models.Requests.AstrologerRequests;
using Quark.CMS.Business.Models.Responses.AstrologerResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class AstrologerService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    StatisticsDataContext statisticsContext)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), IAstrologerService
{
    public async Task<BaseDataTableResponse<List<ListResponseAstrologerItem>>> GetAllPaged(BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<ListResponseAstrologerItem>>();
        
        result.RecordsTotal = _context.Tags.Count();

        var query = (from a in _context.Astrologers
            orderby a.ID
            where !a.IsDeleted
            select new
            {
                ID = a.ID,
                Name = a.Name,
            }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseAstrologerItem
        {
            ID = x.ID,
            Name = x.Name
        }).ToListAsync();

        return result;
    }

    public ServiceResult<CreateAstrologerRequest> Create()
    {
        var result = new ServiceResult<CreateAstrologerRequest>();

        return result;
    }

    public async Task<ServiceResult<CreateAstrologerRequest>> Create(CreateAstrologerRequest request)
    {
        var result = new ServiceResult<CreateAstrologerRequest>();

        var isExist = await _context.Astrologers.AnyAsync(x => x.Name == request.Name);
        if (isExist)
        {
            result.SetError(_localizer["RecordAlreadyExists"]);
            return result;
        }

        var astrologer = new Astrologer
        {
            Name = request.Name,
            ShowInHome = request.ShowInHome
        };
        
        await _context.Astrologers.AddAsync(astrologer);
        await _context.SaveChangesAsync();
        
        return result;    
    }

    public async Task<ServiceResult<UpdateAstrologerRequest>> Update(int id)
    {
        var result = new ServiceResult<UpdateAstrologerRequest>();
        
        var astrologer = await _context.Astrologers.FirstOrDefaultAsync(x => x.ID == id);
        if (astrologer == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var request = new UpdateAstrologerRequest
        {
            ID = id,
            Name = astrologer.Name,
            ShowInHome = astrologer.ShowInHome
        };
        
        result.Data = request;
        return result;    
    }

    public async Task<ServiceResult<UpdateAstrologerRequest>> Update(UpdateAstrologerRequest request)
    {
        var result = new ServiceResult<UpdateAstrologerRequest>();
       
        var isExist = await _context.Astrologers.AnyAsync(x => x.Name == request.Name);
        if (isExist)
        {
            result.SetError(_localizer["RecordAlreadyExists"]);
            return result;
        }
       
        var astrologer = await _context.Astrologers.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (astrologer == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
       
        astrologer.Name = request.Name;
        astrologer.ShowInHome = request.ShowInHome;
       
        _context.Astrologers.Update(astrologer);
        await _context.SaveChangesAsync();
       
        result.Data = new UpdateAstrologerRequest { ID = astrologer.ID,Name = request.Name };
        return result;    
    }

    public async Task<ServiceResult<AstrologerDetailsResponse>> Details(int id)
    {
        var result = new ServiceResult<AstrologerDetailsResponse>();

        var astrologer = await _context.Astrologers.FirstOrDefaultAsync(x => x.ID == id);
        if (astrologer == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var response = new AstrologerDetailsResponse
        {
            ID = astrologer.ID,
            Name = astrologer.Name,
        };
        
        result.Data = response;
        return result;
    }

    public async Task<BaseDataTableResponse<List<DetailsAstrologerNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetAstrologerFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<DetailsAstrologerNewsResponseItem>>();
        
        var query = (from n in statisticsContext.News
                     join s in statisticsContext.NewsStatus on n.StatusID equals s.ID
                     join c in statisticsContext.Categories on n.CategoryID equals c.ID
                     join p in statisticsContext.NewsPhotos on n.ID equals p.NewsID
                     join t in statisticsContext.NewsTypes on n.NewsTypeID equals t.ID
                     join v in statisticsContext.NewsViewsDailies on n.ID equals v.NewsID
                     join a in statisticsContext.Astrologers on n.AuthorID equals a.ID 
                     where p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && a.ID == request.ID
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
        
        result.Data = await query.Select(f => new DetailsAstrologerNewsResponseItem
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
            RelatedNewsCount = statisticsContext.RelatedNews.Count(x => x.NewsID == f.ID),
            TitleTooltip = f.Title,
            StatusId = f.StatusId
        }).ToListAsync();

        #region Impressions için dummy data

        foreach (var item in result.Data)
        {
            if (item.Status == "Yayında")
            {
                // TODO : Viewlar dinamik bir şekilde çekilmeye başlandığında alttaki dummy kodu silinip bu kod açılacak
                var impressionsList = await statisticsContext.NewsViewsMinutelies.Where(x =>
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
            
            if (item.Tags != null && item.Tags.Count() > 0)
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

        var astrologer = await _context.Astrologers.FirstOrDefaultAsync(x => x.ID == id);
        if (astrologer == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        astrologer.IsDeleted = true;

        _context.Astrologers.Update(astrologer);
        await _context.SaveChangesAsync();
        
        return result;
    }
}