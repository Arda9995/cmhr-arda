using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.FtpUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Services;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.SourceRequests;
using Quark.CMS.Business.Models.Requests.TagRequests;
using Quark.CMS.Business.Models.Responses.SourceResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.Common;
using Quark.Data.Models.LogModels;
using Quark.Data.Models.SystemLog;
using Quark.Resources;
using System.Linq;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class SourceService(
    IUrlMappingService urlMappingService,
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    StatisticsDataContext _statisticsContext,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    ISourceCachingService _sourceCachingService,
    FileUpload _fileUpload,
    ISystemLogCachingService systemLogCachingService)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), ISourceService
{
    public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPaged(BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<ListResponseItem>>();

        var query = (from s in _context.Sources
                     select new
                     {
                         ID = s.ID,
                         Name = s.Name,
                         PhotoUrl = s.PhotoUrl
                     }).AsNoTracking().AsQueryable();

        result.RecordsTotal = await query.CountAsync();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Name.ToUpper().Contains(tableRequest.SearchValue.ToUpper()));
        }

        result.RecordsFiltered = await query.CountAsync();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Name = x.Name,
            PhotoUrl = x.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_SourcePhotos)
        }).ToListAsync();

        return result;
    }

    public ServiceResult<CreateSourceRequest> Create()
    {
        var result = new ServiceResult<CreateSourceRequest>();
        var createSource = new CreateSourceRequest
        {
            BaseUrl = _appSettingsKeys.Common_ClientWebSite
        };

        result.Data = createSource;
        return result;
    }

    public async Task<ServiceResult<CreateSourceRequest>> Create(CreateSourceRequest request)
    {
        var result = new ServiceResult<CreateSourceRequest>();

        var isExist = await _context.Sources
            .Where(x => x.Name == request.Name || x.Url == request.Url || x.Breadcrumb == request.Breadcrumb)
            .AnyAsync();

        if (isExist)
        {
            result.SetError(_localizer["RecordAlreadyExists"]);
            result.Data = request;
            return result;
        }

        if (request.Photo != null)
        {
            var uploadResult = _fileUpload.Upload(request.Photo, _appSettingsKeys.CDN_SourcePhotos);
            if (uploadResult.IsSuccess)
            {
                request.PhotoUrl = uploadResult.FileName;
            }
        }

        var source = new Source
        {
            Name = request.Name,
            Url = request.Url,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            Breadcrumb = request.Breadcrumb,
            PhotoUrl = Path.GetFileName(request.PhotoUrl),
            CreatedDate = DateTime.Now
        };

        await _context.Sources.AddAsync(source);
        await _context.SaveChangesAsync();

		await _sourceCachingService.RefreshNewsSourceList();

        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }

    public async Task<ServiceResult<UpdateSourceRequest>> Update(int id)
    {
        var result = new ServiceResult<UpdateSourceRequest>();

        var source = await (from s in _context.Sources
                            where s.ID == id
                            select new UpdateSourceRequest
                            {
                                ID = s.ID,
                                Name = s.Name,
                                PhotoUrl = s.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_SourcePhotos),
                                Breadcrumb = s.Breadcrumb,
                                SeoDescription = s.SeoDescription,
                                SeoTitle = s.SeoTitle,
                                Url = s.Url,
                                BaseUrl = _appSettingsKeys.Common_ClientWebSite
                            }).FirstOrDefaultAsync();

        if (source is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        result.Data = source;
        result.Data.BaseUrl = _appSettingsKeys.Common_ClientWebSite;

        return result;
    }

    public async Task<ServiceResult<UpdateSourceRequest>> Update(UpdateSourceRequest request)
    {
        var result = new ServiceResult<UpdateSourceRequest> { Data = request };

        var source = await _context.Sources.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (source is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var isExist = await _context.Sources
            .Where(x => (x.Name == request.Name || x.Url == request.Url || x.Breadcrumb == request.Breadcrumb) && x.ID != source.ID)
            .AnyAsync();
        if (isExist)
        {
            result.SetError(_localizer["RecordAlreadyExists"]);
            return result;
        }

        if (request.Photo != null)
        {
            var uploadResult = _fileUpload.Upload(request.Photo, _appSettingsKeys.CDN_SourcePhotos);
            if (uploadResult.IsSuccess)
            {
                request.PhotoUrl = uploadResult.FileName;
            }
        }

        if (!string.IsNullOrWhiteSpace(source.Url) &&
            !string.Equals(source.Url, request.Url, StringComparison.OrdinalIgnoreCase))
        {

			var urlMapping = await _context.UrlMappings
				.Where(x => x.NewSlug == source.Url && x.EntityTypeID == (int)EntityTypeEnum.Sources).ToListAsync();

			if (urlMapping != null)
			{
				foreach (var url in urlMapping)
				{
					if (url.OldSlug == request.Url)
					{
						_context.UrlMappings.Remove(url);
					}
					else
					{
						url.NewSlug = request.Url;
						_context.UrlMappings.Update(url);
					}
				}
			}

			await _context.UrlMappings.AddAsync(new UrlMapping
            {
                OldSlug = request.Url,
                NewSlug = source.Url,
                EntityTypeID = (int)EntityTypeEnum.Sources,
                CreatedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
			await urlMappingService.NotifyUrlMappingChanged();
		}

        source.Name = request.Name;
        source.SeoTitle = request.SeoTitle;
        source.SeoDescription = request.SeoDescription;
        source.Url = request.Url;
        source.PhotoUrl = Path.GetFileName(request.PhotoUrl);
        source.Breadcrumb = request.Breadcrumb;

        _context.Update(source);
        await _context.SaveChangesAsync();

        await _sourceCachingService.RefreshNewsSourceList();

        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var result = new ServiceResult();

        var source = await _context.Sources.FirstOrDefaultAsync(x => x.ID == id);

        if (source == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
		var urlMapping = await _context.UrlMappings.Where(x => x.NewSlug == source.Url && x.EntityTypeID == (int)EntityTypeEnum.Sources).ToListAsync();

		if (urlMapping.Count > 0)
		{
			_context.UrlMappings.RemoveRange(urlMapping);
		}

		_context.Sources.Remove(source);
        await _context.SaveChangesAsync();

        await _sourceCachingService.RefreshNewsSourceList();

        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }

    public async Task<ServiceResult<DetailsResponse>> Details(int id)
    {
        var result = new ServiceResult<DetailsResponse>();

        var source = await _context.Sources.FirstOrDefaultAsync(x => x.ID == id);

        if (source is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var response = new DetailsResponse
        {
            ID = id,
            Name = source.Name,
            SeoTitle = source.SeoTitle,
            SeoDescription = source.SeoDescription,
            Breadcrumb = source.Breadcrumb,
            PhotoUrl = source.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                _appSettingsKeys.CDN_MediaGalleryFolder),
            Url = source.Url
        };

        result.Data = response;
        return result;
    }

    public async Task<BaseDataTableResponse<List<DetailsNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetDetailsFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<DetailsNewsResponseItem>>();

        var query = (from n in _statisticsContext.News
                     join s in _statisticsContext.NewsStatus on n.StatusID equals s.ID
                     join c in _statisticsContext.Categories on n.CategoryID equals c.ID
                     join p in _statisticsContext.NewsPhotos on n.ID equals p.NewsID
                     join t in _statisticsContext.NewsTypes on n.NewsTypeID equals t.ID
                     join v in _statisticsContext.NewsViewsDailies on n.ID equals v.NewsID
                     where p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && n.SourceIDs.Contains(request.ID.ToString())
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

        result.Data = await query.Select(f => new DetailsNewsResponseItem
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
                var impressionsList = await _statisticsContext.NewsViewsMinutelies.Where(x =>
                        x.NewsID == item.ID && DateTime.Now.AddMinutes(-15) <= x.ViewsDate)
                    .ToListAsync();

                if (impressionsList.Count > 0 && impressionsList != null)
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

    public async Task<List<SelectListItem>> SearchSources(string search)
    {
        return await _context.Sources
            .Where(x=>x.Name.ToUpper().Contains(search.ToUpper()))
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            }).ToListAsync();
    }

    public async Task<SelectListItem> SearchSourcesByID(int id)
    {
        return await _context.Sources
            .Where(x => x.ID == id)
            .Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name
            }).FirstOrDefaultAsync();
    }
    public async Task<ServiceResult<MergeSourceRequest>> Merge()
    {
        var result = new ServiceResult<MergeSourceRequest>();

        var placeholderList = new List<SelectListItem>();

        var mergeSource = new MergeSourceRequest
        {
            Sources = placeholderList.AddSELECTOption()
        };

        result.Data = mergeSource;
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> SourceSearch(string search)
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var tags = await (from t in _context.Sources
                          where t.Name.ToUpper().Contains(search.ToUpper())
                          select new SelectListItem
                          {
                              Value = t.ID.ToString(),
                              Text = t.Name
                          }).ToListAsync();

        result.Data = tags;

        return result;
    }

    public async Task<ServiceResult<MergeSourceRequest>> Merge(MergeSourceRequest request)
    {
        var result = new ServiceResult<MergeSourceRequest> { Data = new MergeSourceRequest() };
        request.Sources.AddSELECTOption();

        if (request.SelectedTargetSourceID == -1 || request.SelectedSourceSourceID == -1)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var targetSource = await _context.Sources.FirstOrDefaultAsync(x => x.ID == request.SelectedTargetSourceID);
        if (targetSource is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var sourceSource = await _context.Sources.FirstOrDefaultAsync(x => x.ID == request.SelectedSourceSourceID);
        if (sourceSource is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var newsSourceList = await _context.NewsSources.Where(x=>x.SourceID == request.SelectedSourceSourceID).ToListAsync();

        foreach (var newsSource in newsSourceList)
        {
            newsSource.SourceID = request.SelectedTargetSourceID;
            _context.NewsSources.Update(newsSource);
        }

        var log = new SystemLog
        {
            UserID = CurrentUserID,
            IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
            LogTypeID = (int)LogTypesEnum.Source,
            IsSuccess = true,
            Message = targetSource.Name + " ve" + sourceSource.Name+ " birleştirildi.",
            ObjectID = targetSource.ID
        };
        await _context.SystemLogs.AddAsync(log);
        await _context.SaveChangesAsync();
        await systemLogCachingService.RefreshAll();
        await _sourceCachingService.RefreshNewsSourceList();
        return result;
    }
}

