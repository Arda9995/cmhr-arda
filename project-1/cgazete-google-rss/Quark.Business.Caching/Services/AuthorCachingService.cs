using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.AuthorCachingResponses;
using Quark.Business.Caching.Models.Responses.NewsCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Core.Extentions;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class AuthorCachingService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), IAuthorCachingService
{
    #region Refresh Methods

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        try
        {
            await RefreshAuthors();
            await RefreshAuthorDetailCaching();
            await RefreshMainPageAuthorsAll();
            await RefreshAuthorNewsTodayList();
        }
        catch (Exception e)
        {
            result.SetError("Yazar önbellek güncellemesi sırasında bir hata oluştu: " + e.Message);
        }

        return result;
    }

    public async Task<ServiceResult> RefreshAuthors()
    {
        var result = new ServiceResult();
        var authors = await _context.Authors.ToListAsync();
        var jsonData = JsonConvert.SerializeObject(authors);
        await _cache.Set(RedisKeys.AuthorTable, jsonData, RedisTTLSeconds.Authors_TTL);
        return result;
    }

    public async Task<ServiceResult> RefreshAuthorDetailCaching()
    {
        var result = new ServiceResult();

        var astrologieAuthorIds = await _context.AuthorCategories
            .Where(cat => cat.CategoryID == (int)CategoryEnum.Astrologie)
            .Select(cat => cat.AuthorID)
            .Distinct()
            .ToListAsync();

        var query = await (from author in _context.Authors
                where !astrologieAuthorIds.Contains(author.ID)
                orderby author.Order
                let latestNews = _context.NewsPublished
                    .Where(np => np.AuthorID == author.ID)
                    .OrderByDescending(np => np.PublishedDate)
                    .FirstOrDefault()
                let totalNewsCount = _context.NewsPublished
                    .Count(np => np.AuthorID == author.ID)
                select new
                {
                    Author = author,
                    LatestNews = latestNews,
                    TotalNewsCount = totalNewsCount
                })
            .Select(x => new AuthorCachingResponse
            {
                ID = x.Author.ID,
                Firstname = x.Author.Firstname,
                Lastname = x.Author.Lastname,
                Email = x.Author.Email,
                Photo = x.Author.ClichePhoto,
                IsEnabled = x.Author.IsEnabled,
                ShowInHomePage = x.Author.ShowInHome,
                Url = x.Author.Url.TrimStart('/'),
                AuthorSocialFacebook = x.Author.FacebookUrl ?? string.Empty,
                AuthorSocialInstagram = x.Author.InstagramUrl ?? string.Empty,
                AuthorSocialLinkedIn = x.Author.LinkedinUrl ?? string.Empty,
                AuthorSocialMail = x.Author.Email ?? string.Empty,
                AuthorSocialX = x.Author.XUrl ?? string.Empty,
                LastCreatedNewsHeadline = x.LatestNews != null ? x.LatestNews.Title ?? string.Empty : string.Empty,
                LastCreatedNewsSlug = x.LatestNews != null ? x.LatestNews.Slug ?? string.Empty : string.Empty,
                LastCreatedNewsID = x.LatestNews != null ? (int)x.LatestNews.NewsID : 0,
                TotalNewsCount = x.TotalNewsCount
            }).ToListAsync();


        await _cache.Set(RedisKeys.Authors, query);

        return result;
    }

    public async Task<ServiceResult> RefreshMainPageAuthorsAll()
    {
        var result = new ServiceResult();

        var lastPublishedNewsDate = await _context.NewsPublished
            .Where(x => x.AuthorID != null && x.NewsTypeID == (int)NewsTypesEnum.ColumnArticles)
            .MaxAsync(x => x.PublishedDate);

        var query = await (from np in _context.NewsPublished
            join author in _context.Authors on np.AuthorID equals author.ID
            where np.PublishedDate.Date == lastPublishedNewsDate.Date && author.ShowInHome && author.IsEnabled
            orderby author.Order, np.PublishedDate
            select new AuthorCachingResponse
            {
                ID = author.ID,
                Firstname = author.Firstname,
                Lastname = author.Lastname,
                Email = author.Email,
                Photo = author.ClichePhoto,
                IsEnabled = author.IsEnabled,
                ShowInHomePage = author.ShowInHome,
                Url = author.Url.TrimStart('/'),
                AuthorSocialFacebook = author.FacebookUrl ?? string.Empty,
                AuthorSocialInstagram = author.InstagramUrl ?? string.Empty,
                AuthorSocialLinkedIn = author.LinkedinUrl ?? string.Empty,
                AuthorSocialMail = author.Email ?? string.Empty,
                AuthorSocialX = author.XUrl ?? string.Empty,
                LastCreatedNewsHeadline = np.Headline
                                          ?? (np.Title ?? string.Empty),
                LastCreatedNewsSlug = np.Slug ?? string.Empty,
                LastCreatedNewsID = (int)np.NewsID
            }).ToListAsync();

        var totalNewsCount = await (from np in _context.NewsPublished
            join author in _context.Authors on np.AuthorID equals author.ID
            where query.Select(x => x.ID).Contains(author.ID)
            group np by author.ID
            into g
            select new
            {
                AuthorID = g.Key,
                TotalNewsCount = g.Count()
            }).ToListAsync();

        query.ForEach(x =>
            x.TotalNewsCount = totalNewsCount.Where(y => y.AuthorID == x.ID).Select(y => y.TotalNewsCount)
                .FirstOrDefault());

        await _cache.Set(RedisKeys.MainPageAuthors, query);

        return result;
    }

    public async Task<ServiceResult> RefreshAuthorNewsTodayList()
    {
        var result = new ServiceResult();

        var response = new GetAuthorNewsListCacheResponse
        {
            VideoNews = new List<VideoNewsCacheResponse>(),
            CategoryNews = new List<CategoryNewsCacheResponse>(),
            TodaysNewsList = new List<TodaysNewsListCacheResponse>()
        };

        var maxDate = await _context.NewsPublished
            .Where(x => x.AuthorID != null && x.NewsTypeID == (int)NewsTypesEnum.ColumnArticles)
            .MaxAsync(x => x.PublishedDate);

        var query = await (from n in _context.NewsPublished
            join au in _context.Authors on n.AuthorID equals au.ID
            where n.PublishedDate.Date == maxDate.Date &&
                  n.NewsTypeID == (int)NewsTypesEnum.ColumnArticles && au.IsEnabled && (
                      au.ShowDailyColumnArticles || au.ShowInHome)
            orderby au.Order, n.PublishedDate
            select new TodaysNewsListCacheResponse
            {
                NewsID = (int)n.NewsID,
                AuthorName = (au.Firstname ?? string.Empty) + " " + (au.Lastname ?? string.Empty),
                Slug = n.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                Headline = n.Headline ?? n.Title ?? string.Empty,
                Photo = au.ClichePhoto.TrimStart('/')
                    .PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos)
            }).Take(30).ToListAsync();

        var categoryList = await (
            from np in _context.NewsPublished
            join n in _context.News on np.NewsID equals n.ID
            let photo = (from ph in _context.NewsPhotos
                where ph.NewsID == np.NewsID &&
                      (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                       ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                orderby
                    ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
                        ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 :
                        2
                select ph).FirstOrDefault()
            orderby n.ViewCount descending
            select new CategoryNewsCacheResponse
            {
                ID = (int)np.NewsID,
                Headline = np.Headline ?? np.Title,
                UpSubtitle = np.UpSubtitle ?? np.Subtitle ?? np.Content.ExtractTextFromHtml(),
                Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                ImageURL = photo.PhotoUrl.TrimStart('/').PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                    _appSettingsKeys.CDN_MediaGalleryFolder),
            }).Take(5).ToListAsync();

        var videoNewsList = await (
            from np in _context.NewsPublished
            join n in _context.News on np.NewsID equals n.ID
            let photo = (from ph in _context.NewsPhotos
                where ph.NewsID == np.NewsID &&
                      (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                       ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                orderby
                    ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
                        ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 :
                        2
                select ph).FirstOrDefault()
            where n.NewsTypeID == (int)NewsTypesEnum.VideoGallery
            orderby n.ViewCount descending
            select new VideoNewsCacheResponse
            {
                Headline = np.Headline ?? np.Title,
                Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                ImageURL = photo.PhotoUrl.TrimStart('/').PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                    _appSettingsKeys.CDN_MediaGalleryFolder),
                Minute = 0,
            }).Take(3).ToListAsync();

        response.VideoNews = videoNewsList;
        response.CategoryNews = categoryList;
        response.TodaysNewsList = query;

        await _cache.Set(RedisKeys.AuthorNewsTodayList, response);
        return result;
    }

    public async Task<ServiceResult> RefreshAuthorCategoryNewsTodayList(string slug)
    {
        var result = new ServiceResult();

        var response = new GetAuthorNewsListCacheResponse
        {
            VideoNews = new List<VideoNewsCacheResponse>(),
            CategoryNews = new List<CategoryNewsCacheResponse>(),
            TodaysNewsList = new List<TodaysNewsListCacheResponse>()
        };

        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Url == slug);

        if (category == null)
        {
            result.SetError("Kategori Bulunamadı");
            return result;
        }

        var query = await (from n in _context.NewsPublished
            join au in _context.Authors on n.AuthorID equals au.ID
            join ac in _context.AuthorCategories on au.ID equals ac.AuthorID
            where n.PublishedDate.Date == DateTime.Now.Date
                  && ac.CategoryID == category.ID
                  && n.NewsTypeID == (int)NewsTypesEnum.ColumnArticles
            select new TodaysNewsListCacheResponse
            {
                AuthorName = (au.Firstname ?? string.Empty) + " " + (au.Lastname ?? string.Empty),
                Slug = n.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                Headline = n.Headline ?? n.Title ?? string.Empty,
                Photo = au.ClichePhoto.TrimStart('/')
                    .PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos)
            }).Take(30).ToListAsync();

        var categoryList = await (
            from np in _context.NewsPublished
            join n in _context.News on np.NewsID equals n.ID
            let photo = (from ph in _context.NewsPhotos
                where ph.NewsID == np.NewsID &&
                      (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                       ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                orderby
                    ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
                        ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 :
                        2
                select ph).FirstOrDefault()
            orderby n.ViewCount descending
            select new CategoryNewsCacheResponse
            {
                ID = (int)np.NewsID,
                Headline = np.Headline ?? np.Title,
                UpSubtitle = np.UpSubtitle ?? np.Subtitle ?? np.Content.ExtractTextFromHtml(),
                Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                ImageURL = photo.PhotoUrl.TrimStart('/').PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                    _appSettingsKeys.CDN_MediaGalleryFolder),
            }).Take(5).ToListAsync();

        var videoNewsList = await (
            from np in _context.NewsPublished
            join n in _context.News on np.NewsID equals n.ID
            let photo = (from ph in _context.NewsPhotos
                where ph.NewsID == np.NewsID &&
                      (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                       ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                orderby
                    ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
                        ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 :
                        2
                select ph).FirstOrDefault()
            where n.NewsTypeID == (int)NewsTypesEnum.VideoGallery
            orderby n.ViewCount descending
            select new VideoNewsCacheResponse
            {
                Headline = np.Headline ?? np.Title,
                Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                ImageURL = photo.PhotoUrl.TrimStart('/').PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                    _appSettingsKeys.CDN_MediaGalleryFolder),
                Minute = 0,
            }).Take(3).ToListAsync();

        response.CategoryColumnArticleSlug =
            category.Url.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite) + "/yazarlari";
        response.CategorySlug = category.Url;
        response.CategoryName = category.Name;
        response.CategoryBreadcrumb = category.Breadcrumb;
        response.VideoNews = videoNewsList;
        response.CategoryNews = categoryList;
        response.TodaysNewsList = query;

        await _cache.Set(string.Format(RedisKeys.AuthorCategoryNewsTodayList, slug), response);
        return result;
    }

    #endregion

    #region Get Methods

    public async Task<ServiceResult<List<AuthorCachingResponse>>> GetMainPage()
    {
        var result = new ServiceResult<List<AuthorCachingResponse>>();

        result.Data = await _cache.Get<List<AuthorCachingResponse>>(RedisKeys.MainPageAuthors);
        if (result.Data == null)
        {
            await RefreshMainPageAuthorsAll();
            result.Data = await _cache.Get<List<AuthorCachingResponse>>(RedisKeys.MainPageAuthors);
        }

        return result;
    }

    public async Task<ServiceResult<List<AuthorCachingResponse>>> GetAll()
    {
        var result = new ServiceResult<List<AuthorCachingResponse>>();

        result.Data = await _cache.Get<List<AuthorCachingResponse>>(RedisKeys.Authors);
        if (result.Data == null)
        {
            await RefreshAuthorDetailCaching();
            result.Data = await _cache.Get<List<AuthorCachingResponse>>(RedisKeys.Authors);
        }

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetAsSelectList()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var authors = await _cache.Get<List<AuthorCachingResponse>>(RedisKeys.Authors);
        if (authors == null)
        {
            await RefreshAuthorDetailCaching();
            authors = await _cache.Get<List<AuthorCachingResponse>>(RedisKeys.Authors);
        }

        result.Data = authors.Select(x => new SelectListItem
        {
            Text = x.Firstname + " " + x.Lastname,
            Value = x.ID.ToString()
        }).ToList();

        return result;
    }

    public async Task<ServiceResult<GetAuthorNewsListCacheResponse>> GetAuthorNewsTodayList()
    {
        var result = new ServiceResult<GetAuthorNewsListCacheResponse>();

        result.Data = await _cache.Get<GetAuthorNewsListCacheResponse>(RedisKeys.AuthorNewsTodayList);
        if (result.Data == null)
        {
            await RefreshAuthorNewsTodayList();
            result.Data = await _cache.Get<GetAuthorNewsListCacheResponse>(RedisKeys.AuthorNewsTodayList);
        }

        return result;
    }

    public async Task<ServiceResult<GetAuthorNewsListCacheResponse>> GetAuthorCategoryNewsTodayList(string slug)
    {
        var result = new ServiceResult<GetAuthorNewsListCacheResponse>();

        result.Data =
            await _cache.Get<GetAuthorNewsListCacheResponse>(string.Format(RedisKeys.AuthorCategoryNewsTodayList,
                slug));
        if (result.Data == null)
        {
            await RefreshAuthorCategoryNewsTodayList(slug);
            result.Data =
                await _cache.Get<GetAuthorNewsListCacheResponse>(string.Format(RedisKeys.AuthorCategoryNewsTodayList,
                    slug));
        }

        return result;
    }

    public async Task<ServiceResult<List<Author>>> GetAuthors()
    {
        var result = new ServiceResult<List<Author>>();

        result.Data = await _cache.Get<List<Author>>(RedisKeys.AuthorTable);
        if (result.Data == null)
        {
            await RefreshAuthors();
            result.Data = await _cache.Get<List<Author>>(RedisKeys.AuthorTable);
        }

        return result;
    }

    #endregion
}