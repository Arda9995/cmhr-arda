using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.ExcelExport;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.FtpUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.AuthorRequests;
using Quark.CMS.Business.Models.Requests.CommonRequests;
using Quark.CMS.Business.Models.Responses.AuthorResponses;
using Quark.CMS.Business.Models.Responses.CommonResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using System.Linq.Dynamic.Core;
using Quark.Data.Models.Common;

namespace Quark.CMS.Business.Services;

public class AuthorService(
    IUrlMappingService urlMappingService,
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    IAuthorCachingService authorCachingService,
    FileUpload fileUpload,
    INewsCachingService newsCachingService,
    ICategoryCachingService categoryCachingService,
    StatisticsDataContext statisticsContext)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), IAuthorService
{
    public ServiceResult<GetFiltersRequest> GetAllPagedFiltersAsync()
    {
        var result = new ServiceResult<GetFiltersRequest>();
        var filters = new GetFiltersRequest { IsEnabled = true };
        result.Data = filters;
        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest,
        GetFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<ListResponseItem>>();

        result.RecordsTotal = _context.Authors.Count();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        var query = (from a in _context.Authors
            orderby a.Order
            select new
            {
                ID = a.ID,
                Firstname = a.Firstname ?? "",
                Lastname = a.Lastname ?? "",
                Photo = a.Photo ?? "",
                IsEnabled = a.IsEnabled,
                Email = a.Email ?? "",
                IsEnabledText = a.IsEnabled ? enabledText : disabledText,
            }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Firstname.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Photo = (x.Photo ?? "").PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos),
            NameInitials = (!string.IsNullOrEmpty(x.Firstname) && !string.IsNullOrEmpty(x.Lastname))
                ? $"{x.Firstname.First()}{x.Lastname.First()}"
                : "--",
            Name = (x.Firstname + " " + x.Lastname).Trim(),
            Email = x.Email.ToEmptyStringIndicator(),
            IsEnabled = x.IsEnabled,
            IsEnabledText = x.IsEnabledText
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> Create()
    {
        var result = new ServiceResult<CreateUpdateRequest> { Data = new CreateUpdateRequest() };

        var categoriesList = await categoryCachingService.GetCategoriesAsSelectListItems();
        result.Data.Categories = categoriesList.Data;

        result.Data.BaseURL = _appSettingsKeys.Common_ClientWebSite;
        result.Data.ShowInHome = true;
        result.Data.IsEnabled = true;
        result.Data.ShowDailyColumnArticles = true;

        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> CreateAsync(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest> { Data = request };

        var isAuthorExist = await _context.Authors
            .Where(x => x.Firstname == request.Firstname && x.Lastname == request.Lastname).AnyAsync();

        if (isAuthorExist)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        if (request.ProfilePhoto != null)
        {
            var uploadResult = fileUpload.Upload(request.ProfilePhoto, _appSettingsKeys.CDN_AuthorPhotos);
            if (uploadResult.IsSuccess)
            {
                request.ProfilePhotoUrl = uploadResult.FileName;
            }
        }

        if (request.ClichePhoto != null)
        {
            var uploadResult = fileUpload.Upload(request.ClichePhoto, _appSettingsKeys.CDN_AuthorPhotos);
            if (uploadResult.IsSuccess)
            {
                request.ClichePhotoUrl = uploadResult.FileName;
            }
        }

        var author = new Author
        {
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Email = request.Email,
            Photo = request.ProfilePhotoUrl,
            IsEnabled = request.IsEnabled,
            Order = request.Order,
            About = request.About,
            Breadcrumb = request.Breadcrumb,
            Url = $"/{_appSettingsKeys.SlugPrefix_Authors}/" + request.Url,
            ClichePhoto = request.ClichePhotoUrl,
            ColumnName = request.ColumnName,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            ShowInHome = request.ShowInHome,
            FacebookUrl = request.FacebookUrl,
            InstagramUrl = request.InstagramUrl,
            XUrl = request.XUrl,
            YoutubeUrl = request.YoutubeUrl,
            LinkedinUrl = request.LinkedinUrl,
            ShowDailyColumnArticles = request.ShowDailyColumnArticles
        };

        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();

        foreach (var categoryID in request.SelectedCategoryIDs)
        {
            var authorCategory = new AuthorCategory
            {
                AuthorID = author.ID,
                CategoryID = categoryID
            };

            await _context.AuthorCategories.AddAsync(authorCategory);
        }

        await _context.SaveChangesAsync();
        await authorCachingService.RefreshMainPageAuthorsAll();
        await authorCachingService.RefreshAll();

        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(int id)
    {
        var result = new ServiceResult<CreateUpdateRequest>();

        var author = await (from a in _context.Authors
            where a.ID == id
            select new CreateUpdateRequest
            {
                ID = a.ID,
                Firstname = a.Firstname,
                Lastname = a.Lastname,
                Email = a.Email,
                ProfilePhotoUrl =
                    a.Photo.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos),
                IsEnabled = a.IsEnabled,
                Order = a.Order,
                About = a.About,
                Breadcrumb = a.Breadcrumb,
                Url = a.Url,
                ClichePhotoUrl =
                    a.ClichePhoto.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos),
                ColumnName = a.ColumnName,
                SeoTitle = a.SeoTitle,
                SeoDescription = a.SeoDescription,
                ShowInHome = a.ShowInHome,
                FacebookUrl = a.FacebookUrl,
                InstagramUrl = a.InstagramUrl,
                XUrl = a.XUrl,
                YoutubeUrl = a.YoutubeUrl,
                LinkedinUrl = a.LinkedinUrl,
                BaseURL = _appSettingsKeys.Common_ClientWebSite,
                ShowDailyColumnArticles = a.ShowDailyColumnArticles
            }).FirstOrDefaultAsync();

        if (author is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var categoriesList = await categoryCachingService.GetCategoriesAsSelectListItems();
        author.Categories = categoriesList.Data;

        author.SelectedCategoryIDs = await _context.AuthorCategories.Where(x => x.AuthorID == id)
            .Select(x => x.CategoryID).ToListAsync();

        result.Data = author;

        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> UpdateAsync(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest> { Data = request };

        var author = await _context.Authors.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (author is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var isAuthorExist = await _context.Authors.Where(x =>
            x.Firstname == request.Firstname && x.Lastname == request.Lastname && x.IsEnabled == request.IsEnabled &&
            x.Email == request.Email && x.ID != request.ID).AnyAsync();

        if (isAuthorExist)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        if (request.ProfilePhoto != null)
        {
            var uploadResult = fileUpload.Upload(request.ProfilePhoto, _appSettingsKeys.CDN_AuthorPhotos);
            if (uploadResult.IsSuccess)
            {
                author.Photo = uploadResult.FileName;
            }
        }

        if (request.ClichePhoto != null)
        {
            var uploadResult = fileUpload.Upload(request.ClichePhoto, _appSettingsKeys.CDN_AuthorPhotos);
            if (uploadResult.IsSuccess)
            {
                author.ClichePhoto = uploadResult.FileName;
            }
        }

        if (!string.IsNullOrWhiteSpace(author.Url) &&
            !string.Equals(author.Url, request.Url, StringComparison.OrdinalIgnoreCase))
        {
            var urlMapping = await _context.UrlMappings
                .Where(x => x.NewSlug == author.Url && x.EntityTypeID == (int)EntityTypeEnum.Authors).ToListAsync();

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
                OldSlug = author.Url,
                NewSlug = request.Url,
                EntityTypeID = (int)EntityTypeEnum.Authors,
                CreatedDate = DateTime.Now,
            });

            await _context.SaveChangesAsync();
            await urlMappingService.NotifyUrlMappingChanged();
        }

        author.Email = request.Email;
        author.Firstname = request.Firstname;
        author.Lastname = request.Lastname;
        author.IsEnabled = request.IsEnabled;
        author.Order = request.Order;
        author.About = request.About;
        author.Breadcrumb = request.Breadcrumb;
        author.Url = request.Url;
        author.ColumnName = request.ColumnName;
        author.SeoTitle = request.SeoTitle;
        author.SeoDescription = request.SeoDescription;
        author.ShowInHome = request.ShowInHome;
        author.XUrl = request.XUrl;
        author.YoutubeUrl = request.YoutubeUrl;
        author.FacebookUrl = request.FacebookUrl;
        author.InstagramUrl = request.InstagramUrl;
        author.LinkedinUrl = request.LinkedinUrl;
        author.ShowDailyColumnArticles = request.ShowDailyColumnArticles;

        var authorCategories = await _context.AuthorCategories.Where(x => x.AuthorID == author.ID).ToListAsync();
        _context.AuthorCategories.RemoveRange(authorCategories);

        foreach (var categoryID in request.SelectedCategoryIDs)
        {
            var authorCategory = new AuthorCategory
            {
                AuthorID = author.ID,
                CategoryID = categoryID
            };

            await _context.AuthorCategories.AddAsync(authorCategory);
        }

        var slug = author.Url.Split('/').LastOrDefault();

        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
        await authorCachingService.RefreshAll();
        await newsCachingService.RefreshAuthorDetailLastColumnArticles(slug);
        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var result = new ServiceResult();

        var author = await _context.Authors.FirstOrDefaultAsync(x => x.ID == id);
        if (author is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var authorCategories = await _context.AuthorCategories.Where(x => x.AuthorID == author.ID).ToListAsync();
        _context.AuthorCategories.RemoveRange(authorCategories);
        author.IsEnabled = false;
        _context.Authors.Update(author);

        var urlMapping = await _context.UrlMappings
            .Where(x => x.NewSlug == author.Url && x.EntityTypeID == (int)EntityTypeEnum.Authors).ToListAsync();

        if (urlMapping.Count > 0)
        {
            _context.UrlMappings.RemoveRange(urlMapping);
        }

        await _context.SaveChangesAsync();
        await authorCachingService.RefreshAll();
        await urlMappingService.NotifyUrlMappingChanged();
        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }

    public async Task<ServiceResult<DetailsAuthorRequest>> DetailsAsync(int id)
    {
        var result = new ServiceResult<DetailsAuthorRequest>();

        result.Data = await (from a in _context.Authors
            where a.ID == id
            select new DetailsAuthorRequest
            {
                ID = a.ID,
                Firstname = a.Firstname,
                Lastname = a.Lastname,
                Fullname = a.Firstname + " " + a.Lastname,
                Email = a.Email,
                Photo = a.Photo.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos),
                IsEnabled = a.IsEnabled,
                ClichePhoto =
                    a.ClichePhoto.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos)
            }).FirstOrDefaultAsync();
        return result;
    }

    public async Task<ServiceResult<ExcelExportResponse>> DownloadExcelExportAsync(ExcelExportRequest request)
    {
        var result = new ServiceResult<ExcelExportResponse>();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        var query = (from t in _context.Authors
            orderby t.Firstname
            select new
            {
                ID = t.ID,
                Firstname = t.Firstname,
                Lastname = t.Lastname,
                Email = t.Email,
                IsEnabled = t.IsEnabled,
            }).AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(x =>
                x.Firstname.ToLower().Contains(request.SearchValue.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.SortColumn) || !string.IsNullOrEmpty(request.SortDirection))
        {
            query = query.OrderBy(request.SortColumn + " " + request.SortDirection);
        }

        var list = await query.Select(x => new ListResponseExcelItem
        {
            ID = x.ID,
            Name = x.Firstname + " " + x.Lastname,
            Email = x.Email,
            IsEnabledText = x.IsEnabled ? enabledText : disabledText,
        }).ToListAsync();

        result.Data = new ExcelExportResponse
        {
            FileBytes = list.ExportToExcel(),
            FileName = "Yazarlar.xlsx"
        };

        return result;
    }

    public async Task<BaseDataTableResponse<List<DetailResponseNewsItem>>> GetNewsPaged(
        BaseDataTableRequest tableRequest, DetailsAuthorRequest request)
    {
        var result = new BaseDataTableResponse<List<DetailResponseNewsItem>>();

        var query = (from n in statisticsContext.News
            join s in statisticsContext.NewsStatus on n.StatusID equals s.ID
            join c in statisticsContext.Categories on n.CategoryID equals c.ID
            join p in statisticsContext.NewsPhotos on n.ID equals p.NewsID
            join t in statisticsContext.NewsTypes on n.NewsTypeID equals t.ID
            join v in statisticsContext.NewsViewsDailies on n.ID equals v.NewsID
            where p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && n.AuthorID == request.ID
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

        result.Data = await query.Select(f => new DetailResponseNewsItem
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
}