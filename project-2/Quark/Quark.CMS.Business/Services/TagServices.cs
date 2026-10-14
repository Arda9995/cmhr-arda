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
using Quark.CMS.Business.Models.Requests.TagRequests;
using Quark.CMS.Business.Models.Responses.CommonResponses;
using Quark.CMS.Business.Models.Responses.TagResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.FtpUpload;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Quark.Data.Models.LogModels;
using Quark.Data.Models.SystemLog;
using Quark.Data.Models.Common;
using Azure.Core;

namespace Quark.CMS.Business.Services;

public class TagServices : BaseService, ITagService
{
    private readonly ITagCachingService _tagCachingService;
    private readonly StatisticsDataContext _statisticsContext;
    private readonly FileUpload _fileUpload;
    private readonly ISystemLogCachingService _systemLogCachingService;
    private readonly INewsCachingService _newsCachingService;
    private readonly IUrlMappingService _urlMappingService;
	public TagServices(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, ITagCachingService tagCachingService, StatisticsDataContext statisticsContext, FileUpload fileUpload, ISystemLogCachingService systemLogCachingService, INewsCachingService newsCachingService, IUrlMappingService urlMappingService = null) : base(httpContext, context, localizer, appSettingsKeys, cache)
	{
		_tagCachingService = tagCachingService;
		_statisticsContext = statisticsContext;
		_fileUpload = fileUpload;
		_systemLogCachingService = systemLogCachingService;
		_appSettingsKeys = appSettingsKeys;
		_newsCachingService = newsCachingService;
		_urlMappingService = urlMappingService;
	}

	public ServiceResult<CreateTagRequest> Create()
    {
        var result = new ServiceResult<CreateTagRequest>();
        var createTag = new CreateTagRequest();
        
        result.Data = createTag;
        result.Data.BaseURL = _appSettingsKeys.Common_ClientWebSite;

        return result;
    }

    public async Task<ServiceResult<CreateTagRequest>> Create(CreateTagRequest request)
    {
        var result = new ServiceResult<CreateTagRequest> { Data = request };

        var isTagExist = await _context.Tags.Where((x => x.Text == request.Text.Trim() || x.Url == request.Url.Trim() || x.Breadcrumb == request.Breadcrumb)).AnyAsync();

        if (isTagExist)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
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
        
        var tag = new Tag
        {
            ID = (int)request.ID,
            Text = request.Text,
            Url = request.Url,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            Breadcrumb = request.Breadcrumb,
            PhotoUrl = Path.GetFileName(request.PhotoUrl),
            CreatedDate = DateTime.Now
        };

        await _context.Tags.AddAsync(tag);
        await _context.SaveChangesAsync();

        var log = new SystemLog
        {
            UserID = CurrentUserID,
            IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
            LogTypeID = (int)LogTypesEnum.Tag,
            IsSuccess = true,
            Message = tag.Text + "oluşturuldu.",
            ObjectID = tag.ID
        };
        await _context.SystemLogs.AddAsync(log);
        await _context.SaveChangesAsync();
		await _systemLogCachingService.RefreshAll();
        await _tagCachingService.RefreshAll();
        await _tagCachingService.InsertTagElastic(tag);

        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult> Delete(long id)
    {
        var result = new ServiceResult();

        var tag = await _context.Tags.FirstOrDefaultAsync(x => x.ID == id);

        if (tag == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var newsList = await (from n in _context.News
            join nt in _context.NewsTags on n.ID equals nt.NewsID
            where nt.TagID == id
            select n).ToListAsync();

        if (newsList.Count > 0)
        {
            foreach (var news in newsList)
            {
                var tagsList = news.Tags.Split(',').ToList();
            
                tagsList.Remove(tag.Text);

                string tags = string.Join(",", tagsList);
                news.Tags = tags;

                if (news.StatusID == (int)NewsStatusEnum.Published)
                {
                    var newsPublished = await _context.NewsPublished.FirstOrDefaultAsync(x => x.NewsID == news.ID);

                    var tagsListPublished = newsPublished.Tags.Split(',').ToList();

                    tagsListPublished.Remove(tag.Text);

                    string publishedTags = string.Join(",", tagsListPublished);
                    newsPublished.Tags = publishedTags;
                
                    _context.NewsPublished.Update(newsPublished);
                }
            }
        
            _context.News.UpdateRange(newsList);
        }
        
        var newsTags = await _context.NewsTags.Where(x => x.TagID == id).ToListAsync();

        if (newsTags.Count > 0)
        {
            _context.NewsTags.RemoveRange(newsTags);
        }
        
        _context.Tags.Remove(tag);

        var urlMapping = await _context.UrlMappings.Where(x => x.NewSlug == tag.Url && x.EntityTypeID == (int)EntityTypeEnum.Tags).ToListAsync();

        if (urlMapping.Count > 0)
        {
            _context.UrlMappings.RemoveRange(urlMapping);
        }

        var log = new SystemLog
        {
            UserID = CurrentUserID,
            IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
            LogTypeID = (int)LogTypesEnum.Tag,
            IsSuccess = true,
            Message = tag.Text + "silindi.",
            ObjectID = tag.ID
        };
        await _context.SystemLogs.AddAsync(log);
        await _context.SaveChangesAsync();
		await _urlMappingService.NotifyUrlMappingChanged();
		await _tagCachingService.RefreshAll();
        await _systemLogCachingService.RefreshAll();
        await _newsCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }

    public async Task<ServiceResult<DetailsResponse>> Details(long id)
    {
        var result = new ServiceResult<DetailsResponse>();

        var tag = await _context.Tags.FirstOrDefaultAsync(x => x.ID == id);

        if (tag is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
        
        var response = new DetailsResponse
        {
            ID = id,
            TagName = tag.Text,
            PhotoUrl = tag.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_TagPhotos)
        };

        result.Data = response;
        return result;
    }

    public async Task<BaseDataTableResponse<List<DetailsTagsNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetTagFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<DetailsTagsNewsResponseItem>>();
        
        var query = (from n in _statisticsContext.News
                     join s in _statisticsContext.NewsStatus on n.StatusID equals s.ID
                     join c in _statisticsContext.Categories on n.CategoryID equals c.ID
                     join p in _statisticsContext.NewsPhotos on n.ID equals p.NewsID
                     join t in _statisticsContext.NewsTypes on n.NewsTypeID equals t.ID
                     join v in _statisticsContext.NewsViewsDailies on n.ID equals v.NewsID
                     join nt in _statisticsContext.NewsTags on n.ID equals nt.NewsID 
                     where p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && nt.TagID == request.ID
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
        
        result.Data = await query.Select(f => new DetailsTagsNewsResponseItem
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

    public async Task<ServiceResult<MergeTagRequest>> Merge()
    {
        var result = new ServiceResult<MergeTagRequest>();

        var placeholderList = new List<SelectListItem>();
        
        var mergeTag = new MergeTagRequest
        {
            Tags = placeholderList.AddSELECTOption()
        };

        result.Data = mergeTag;    
        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> TagSearch(string search)
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var tags = await (from t in _context.Tags
            where t.Text.ToUpper().Contains(search.ToUpper())
            select new SelectListItem
            {
                Value = t.ID.ToString(),
                Text = t.Text
            }).ToListAsync();

        result.Data = tags;
        
        return result;
    }

    public async Task<ServiceResult<SelectListItem>> TagSearchByID(int id)
    {
        var result = new ServiceResult<SelectListItem>();

        var tags = await (from t in _context.Tags
                          where t.ID == id
                          select new SelectListItem
                          {
                              Value = t.ID.ToString(),
                              Text = t.Text
                          }).FirstOrDefaultAsync();

        result.Data = tags;
        return result;
    }

    public async Task<ServiceResult<MergeTagRequest>> Merge(MergeTagRequest request)
    {
        var result = new ServiceResult<MergeTagRequest> { Data = new MergeTagRequest()};
        request.Tags.AddSELECTOption();

        if (request.SelectedTargetTagID == -1 || request.SelectedSourceTagID == -1)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var targetTag = await _context.Tags.FirstOrDefaultAsync(x => x.ID == request.SelectedTargetTagID);
        if (targetTag is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
        
        var sourceTag = await _context.Tags.FirstOrDefaultAsync(x => x.ID == request.SelectedSourceTagID);
        if (sourceTag is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
        
        var newsList = await (from n in _context.News
            join nt in _context.NewsTags on n.ID equals nt.NewsID
            where nt.TagID == request.SelectedSourceTagID
            select n).ToListAsync();

        foreach (var news in newsList)
        {
            var tagList = news.Tags.Split(',').ToList();
            
            var newsTag = await _context.NewsTags.FirstOrDefaultAsync(x => x.TagID == sourceTag.ID && x.NewsID == news.ID);
            
            if (tagList.Contains(targetTag.Text))
            {
                tagList.Remove(sourceTag.Text);

                string updatedList = string.Join(",", tagList);
                
                news.Tags = updatedList;
            }
            else
            {
                var index = tagList.IndexOf(sourceTag.Text);
                
                tagList[index] = targetTag.Text.ToKeyword();
                string updatedList  = string.Join(",", tagList);
                
                news.Tags = updatedList;

                var NewsTag = new NewsTag
                {
                    NewsID = newsTag.NewsID,
                    TagID = (int)targetTag.ID
                };
                
                await _context.NewsTags.AddAsync(NewsTag);
            }
            
            if (news.StatusID == (int)NewsStatusEnum.Published)
            {
                var newsPublished = await _context.NewsPublished.FirstOrDefaultAsync(x => x.NewsID == news.ID);

                newsPublished.Tags = news.Tags;
                
                _context.NewsPublished.Update(newsPublished);
            }
            _context.Remove(newsTag);
            await _context.SaveChangesAsync();
        }
        
        _context.Remove(sourceTag);
        
        var log = new SystemLog
        {
            UserID = CurrentUserID,
            IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
            LogTypeID = (int)LogTypesEnum.Tag,
            IsSuccess = true,
            Message = targetTag.Text + " ve" + sourceTag.Text + " birleştirildi.",
            ObjectID = targetTag.ID
        };
        await _context.SystemLogs.AddAsync(log);
        await _context.SaveChangesAsync();
        await _systemLogCachingService.RefreshAll();
        await _newsCachingService.RefreshAllNewsTags();

        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPaged(BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<ListResponseItem>>();
        
        result.RecordsTotal = _context.Tags.Count();

        var query = (from t in _context.Tags
            orderby t.Text
            select new
            {
                ID = t.ID,
                Text = t.Text,
                PhotoUrl = t.PhotoUrl
            }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Text.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Name = x.Text,
            URL = _appSettingsKeys.Tags_PrefixUrl + x.Text,
            PhotoUrl = x.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_TagPhotos)
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<ExcelExportResponse>> DownloadExcelExportAsync(ExcelExportRequest request)
    {
        var result = new ServiceResult<ExcelExportResponse>();

        var query = (from t in _context.Tags
            orderby t.Text
            select new
            {
                ID = t.ID,
                Text = t.Text,
            }).AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(x =>
                x.Text.ToLower().Contains(request.SearchValue.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.SortColumn) || !string.IsNullOrEmpty(request.SortDirection))
        {
            query = query.OrderBy(request.SortColumn + " " + request.SortDirection);
        }

        var list = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Name = x.Text,
        }).ToListAsync();

        result.Data = new ExcelExportResponse
        {
            FileBytes = list.ExportToExcel(),
            FileName = "Etiketler.xlsx"
        };

        return result;
    }
    
    public async Task<ServiceResult<UpdateTagRequest>> Update(long id)
    {
        var result = new ServiceResult<UpdateTagRequest>();

        var tag = await (from t in _context.Tags
            where t.ID == id
            select new UpdateTagRequest
            {
                ID = t.ID,
                Text = t.Text,
                Url = t.Url,
                SeoTitle = t.SeoTitle,
                SeoDescription = t.SeoDescription,
                Breadcrumb = t.Breadcrumb,
                PhotoUrl = t.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_TagPhotos)
            }).FirstOrDefaultAsync();

        if (tag is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
        
        result.Data = tag;
        result.Data.BaseURL = _appSettingsKeys.Common_ClientWebSite;

        return result;
    }

    public async Task<ServiceResult<UpdateTagRequest>> Update(UpdateTagRequest request)
    {
        var result = new ServiceResult<UpdateTagRequest> { Data = request };

        var tag = await _context.Tags.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (tag is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var isTagExist = await _context.Tags.Where(x => (x.Text == request.Text ||x.Url == request.Url || x.Breadcrumb == request.Breadcrumb) && x.ID != request.ID).AnyAsync();

        if (isTagExist)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }
        
        var newsList = await (from n in _context.News
            join nt in _context.NewsTags on n.ID equals nt.NewsID
            where nt.TagID == request.ID
            select n).ToListAsync();

        foreach (var news in newsList)
        {
            var tagList = news.Tags.Split(',').ToList();
            var index = tagList.IndexOf(tag.Text);

            tagList[index] = request.Text.ToKeyword();
            var updatedList = news.Tags = string.Join(",", tagList);
            
            news.Tags = updatedList;
            
            if (news.StatusID == (int)NewsStatusEnum.Published)
            {
                var newsPublished = await _context.NewsPublished.FirstOrDefaultAsync(x => x.NewsID == news.ID);

                newsPublished.Tags = updatedList;
                
                _context.NewsPublished.Update(newsPublished);
            }
        }

        _context.News.UpdateRange(newsList);
        
        if (request.Photo != null)
        {
            var uploadResult = _fileUpload.Upload(request.Photo, _appSettingsKeys.CDN_SourcePhotos);
            if (uploadResult.IsSuccess)
            {
                request.PhotoUrl = uploadResult.FileName;
            }
        }

        if (!string.IsNullOrWhiteSpace(tag.Url) &&
    !string.Equals(tag.Url, request.Url, StringComparison.OrdinalIgnoreCase))
        {
			var urlMapping = await _context.UrlMappings
				.Where(x => x.NewSlug == tag.Url && x.EntityTypeID == (int)EntityTypeEnum.Tags).ToListAsync();

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
                NewSlug = tag.Url,
                OldSlug = request.Url,
                EntityTypeID = (int)EntityTypeEnum.Tags,
                CreatedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
			await _urlMappingService.NotifyUrlMappingChanged();
		}

        tag.Text = request.Text.ToKeyword();
        tag.Url = request.Url;
        tag.SeoTitle = request.SeoTitle;
        tag.SeoDescription = request.SeoDescription;
        tag.Breadcrumb = request.Breadcrumb;
        tag.PhotoUrl = Path.GetFileName(request.PhotoUrl);
        
        _context.Tags.Update(tag);

        var log = new SystemLog
        {
            UserID = CurrentUserID,
            IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
            LogTypeID = (int)LogTypesEnum.Tag,
            IsSuccess = true,
            Message = tag.Text + " güncellendi.",
            ObjectID = tag.ID
        };
        await _context.SystemLogs.AddAsync(log);
        await _context.SaveChangesAsync();
        await _tagCachingService.RefreshAll();
        await _tagCachingService.UpdateTagElastic(tag);
        await _systemLogCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }
    
}