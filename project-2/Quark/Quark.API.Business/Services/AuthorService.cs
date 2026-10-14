using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.AuthorResponses;
using Quark.API.Business.Models.Responses.NewsResponses;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Requests.AuthorCachingRequests;
using Quark.Business.Caching.Services;
using Quark.Business.Common.BaseServices;
using Quark.Core.Extentions;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;

namespace Quark.API.Business.Services;

public class AuthorService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    IAuthorCachingService authorCachingService,
    INewsCachingService newsCachingService)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), IAuthorService
{
    public async Task<ServiceResult<List<GetAuthorListResponse>>> GetAuthorList()
    {
        var result = new ServiceResult<List<GetAuthorListResponse>>();

        var authors = await authorCachingService.GetAll();

        if (!authors.IsSuccess)
        {
            result.SetError("Yazar Bulunamadı");
            return result;
        }

        var authorList = authors?.Data?
            .Where(x => x != null && x.IsEnabled)
            .Select(x => new GetAuthorListResponse
            {
                ID = x.ID,
                FullName = $"{x.Firstname ?? string.Empty} {x.Lastname ?? string.Empty}".Trim(),
                ImageURL = string.IsNullOrWhiteSpace(x.Photo)
                    ? string.Empty
                    : x.Photo.TrimStart('/')
                        .PrepareCDNUrl(
                            _appSettingsKeys.CDN_BaseUrl,
                            _appSettingsKeys.CDN_AuthorPhotos
                        ),
                Description = x.Description,
                TotalNewsCount = x.TotalNewsCount ?? 0,
                Slug = string.IsNullOrWhiteSpace(x.Url)
                    ? string.Empty
                    : _appSettingsKeys.Common_ClientWebSite + x.Url.TrimStart('/'),
                LastCreatedNewsHeadline = x.LastCreatedNewsHeadline ?? string.Empty,
                LastCreatedNewsSlug = string.IsNullOrWhiteSpace(x.LastCreatedNewsSlug)
                    ? string.Empty
                    : _appSettingsKeys.Common_ClientWebSite + x.LastCreatedNewsSlug.TrimStart('/'),
                LastCreatedNewsID = x.LastCreatedNewsID
            })
            .ToList() ?? new List<GetAuthorListResponse>();


        result.Data = authorList;
        return result;
    }

    public async Task<ServiceResult<AuthorDetailResponse>> GetAuthorDetail(string slug)
    {
        var result = new ServiceResult<AuthorDetailResponse>();

        var response = await newsCachingService.GetAuthorDetailLastColumnArticles(slug);

        if (!response.IsSuccess)
        {
            result.SetError("Yazar Detayı Bulunamadı");
            return result;
        }

        result.Data = new AuthorDetailResponse
        {
            ID = response.Data.ID,
            FullName = response.Data.FullName,
            Photo = response.Data.Photo,
            IsEnabled = response.Data.IsEnabled,
            SocialFacebook = response.Data.SocialFacebook,
            SocialInstagram = response.Data.SocialInstagram,
            SocialLinkedIn = response.Data.SocialLinkedIn,
            SocialMail = response.Data.SocialMail,
            SocialX = response.Data.SocialX,
            Slug = response.Data.Slug,
            AuthorsLastColumnArticles = response.Data.AuthorsLastColumnArticles.Select(x =>
                new AuthorsLastColumnArticles
                {
                    UpSubtitle = x.UpSubtitle,
                    Headline = x.Headline,
                    Slug = x.Slug,
                    PublishDate = x.PublishDate,
                    NewsID = x.NewsID,
                }).ToList()
        };

        return result;
    }

    public async Task<ServiceResult<List<GetAuthorListResponse>>> GetHomePageAuthorList()
    {
        var result = new ServiceResult<List<GetAuthorListResponse>>();

        var authors = await authorCachingService.GetMainPage();

        if (!authors.IsSuccess)
        {
            result.SetError("Yazar Bulunamadı");
            return result;
        }

        var authorList = authors.Data.Where(x => x.IsEnabled && x.ShowInHomePage).Select(x => new GetAuthorListResponse
        {
            ID = x.ID,
            FullName = x.Firstname + " " + x.Lastname,
            ImageURL = x.Photo.TrimStart('/')
                .PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos),
            Description = x.Description,
            TotalNewsCount = x.TotalNewsCount ?? 0,
            Slug = x.Url,
            LastCreatedNewsSlug = x.LastCreatedNewsSlug != null
                ? _appSettingsKeys.Common_ClientWebSite + x.LastCreatedNewsSlug.TrimStart('/')
                : string.Empty,
            LastCreatedNewsHeadline = x.LastCreatedNewsHeadline ?? string.Empty,
            LastCreatedNewsID = x.LastCreatedNewsID,
        }).ToList();

        result.Data = authorList;
        return result;
    }

    public async Task<ServiceResult<GetAuthorNewsListResponse>> GetAuthorNewsTodayList()
    {
        var result = new ServiceResult<GetAuthorNewsListResponse>
        {
            Data = new GetAuthorNewsListResponse
            {
                TodaysNewsList = new List<TodaysNewsList>(),
            }
        };

        var authorNewsTodayData = await authorCachingService.GetAuthorNewsTodayList();

        if (!authorNewsTodayData.IsSuccess)
        {
            result.SetError("Yazar Bulunamadı");
            return result;
        }

        var todayNewsList = authorNewsTodayData.Data.TodaysNewsList.Select(x => new TodaysNewsList
        {
            AuthorName = x.AuthorName,
            Slug = x.Slug,
            Headline = x.Headline,
            Photo = x.Photo,
            NewsID = x.NewsID,
            PublishedDate = x.PublishedDate
        });

        result.Data.TodaysNewsList = todayNewsList.ToList();
        return result;
    }

    public async Task<ServiceResult<object>> GetFilteredArticlesWithPagination(int authorId, string dateRange,
        int page = 1, int pageSize = 40)
    {
        var result = new ServiceResult<object>();

        try
        {
            // Parse date range
            var (startDate, endDate) = ParseDateRange(dateRange);

            // Get articles
            var articlesResult = await GetAuthorArticlesByDateRange(authorId, startDate, endDate, page, pageSize);
            if (!articlesResult.IsSuccess)
            {
                result.SetError(articlesResult.Message);
                return result;
            }

            // Get total count
            var totalCountResult = await GetAuthorArticlesCount(authorId, startDate, endDate);
            if (!totalCountResult.IsSuccess)
            {
                result.SetError(totalCountResult.Message);
                return result;
            }

            // Create response object
            var response = new
            {
                articles = articlesResult.Data,
                totalCount = totalCountResult.Data,
                currentPage = page,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling(totalCountResult.Data / (double)pageSize),
                hasMore = (page * pageSize) < totalCountResult.Data
            };

            result.Data = response;
        }
        catch (Exception ex)
        {
            result.SetError(ex.Message);
        }

        return result;
    }

    public async Task<ServiceResult<GetAuthorNewsListResponse>> GetAuthorCategoryNewsTodayList(string slug)
    {
        var result = new ServiceResult<GetAuthorNewsListResponse>
        {
            Data = new GetAuthorNewsListResponse
            {
                VideoNews = new List<VideoNews>(),
                CategoryNews = new List<CategoryNews>(),
                TodaysNewsList = new List<TodaysNewsList>()
            }
        };

        var authorNewsTodayData = await authorCachingService.GetAuthorCategoryNewsTodayList(slug);

        if (!authorNewsTodayData.IsSuccess)
        {
            result.SetError("Yazar Bulunamadı");
            return result;
        }

        var responseModel = new GetAuthorNewsListResponse
        {
            CategoryBreadcrumb = authorNewsTodayData.Data.CategoryBreadcrumb,
            CategoryColumnArticleSlug = authorNewsTodayData.Data.CategoryColumnArticleSlug,
            CategoryName = authorNewsTodayData.Data.CategoryName,
            CategorySlug = authorNewsTodayData.Data.CategorySlug,
            CategoryNews = new List<CategoryNews>(),
            TodaysNewsList = new List<TodaysNewsList>(),
            VideoNews = new List<VideoNews>()
        };

        var todayNewsList = authorNewsTodayData.Data.TodaysNewsList.Select(x => new TodaysNewsList
        {
            AuthorName = x.AuthorName,
            Slug = x.Slug,
            Headline = x.Headline,
            Photo = x.Photo,
            NewsID = x.NewsID,
            PublishedDate = x.PublishedDate
        });

        responseModel.TodaysNewsList = todayNewsList.ToList();

        result.Data = responseModel;
        return result;
    }

    #region Helper

    private async Task<ServiceResult<List<AuthorsLastColumnArticles>>> GetAuthorArticlesByDateRange(int authorId,
        DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 40)
    {
        var result = new ServiceResult<List<AuthorsLastColumnArticles>>();

        try
        {
            var query = from np in _context.NewsPublished
                join a in _context.Authors on np.AuthorID equals a.ID
                where np.AuthorID == authorId && np.NewsTypeID == (int)NewsTypesEnum.ColumnArticles
                select new
                {
                    NewsPublished = np,
                    Author = a
                };


            if (startDate.HasValue)
            {
                query = query.Where(x => x.NewsPublished.PublishedDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.NewsPublished.PublishedDate.Date <= endDate.Value.Date);
            }


            var skip = (page - 1) * pageSize;

            var articles = await query
                .OrderByDescending(x => x.NewsPublished.PublishedDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new AuthorsLastColumnArticles
                {
                    UpSubtitle = x.NewsPublished.UpSubtitle ?? x.NewsPublished.Subtitle,
                    Headline = x.NewsPublished.Headline ?? x.NewsPublished.Title,
                    Slug = _appSettingsKeys.Common_ClientWebSite + x.NewsPublished.Slug.TrimStart('/'),
                    PublishDate = x.NewsPublished.PublishedDate.ToFormattedDate(),
                    NewsID = (int)x.NewsPublished.NewsID,
                })
                .ToListAsync();

            result.Data = articles;
        }
        catch (Exception ex)
        {
            result.SetError(ex.Message);
        }

        return result;
    }

    private async Task<ServiceResult<int>> GetAuthorArticlesCount(int authorId, DateTime? startDate, DateTime? endDate)
    {
        var result = new ServiceResult<int>();

        try
        {
            var query = from np in _context.NewsPublished
                where np.AuthorID == authorId && np.NewsTypeID == (int)NewsTypesEnum.ColumnArticles
                select np;

            // Apply date filtering if dates are provided
            if (startDate.HasValue)
            {
                query = query.Where(x => x.PublishedDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.PublishedDate.Date <= endDate.Value.Date);
            }

            result.Data = await query.CountAsync();
        }
        catch (Exception ex)
        {
            result.SetError(ex.Message);
            result.Data = 0;
        }

        return result;
    }

    private (DateTime? startDate, DateTime? endDate) ParseDateRange(string dateRange)
    {
        DateTime? startDate = null;
        DateTime? endDate = null;

        if (!string.IsNullOrEmpty(dateRange))
        {
            // Split by "to" for the format "01.02.2024 to 02.02.2024"
            string[] dates = dateRange.Split(new string[] { " - " }, StringSplitOptions.None);

            if (dates.Length == 2)
            {
                // Parse dates in format dd.MM.yyyy
                if (DateTime.TryParseExact(dates[0].Trim(), "dd.MM.yyyy",
                        System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                        out DateTime parsedStartDate))
                {
                    startDate = parsedStartDate;
                }

                if (DateTime.TryParseExact(dates[1].Trim(), "dd.MM.yyyy",
                        System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,
                        out DateTime parsedEndDate))
                {
                    endDate = parsedEndDate;
                }
            }
        }

        return (startDate, endDate);
    }

    #endregion
}