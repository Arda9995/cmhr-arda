using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Responses.HomeResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;
using Category = GoogleTrendsApi.Category;

namespace Quark.CMS.Business.Services;

public class HomeService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache,
    INewsCachingService newsCachingService,
    StatisticsDataContext statisticsContext,
    ISystemLogCachingService _systemLogCachingService,
    IGoogleTrendsService _googleTrendsService, ILogger<HomeService> logger)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), IHomeService
{
    public async Task<ServiceResult<HomeResponse>> Index()
    {
        var result = new ServiceResult<HomeResponse>();
        result.Data = new HomeResponse();

        //var totalPublishedNewsCount = await newsCachingService.GetTotalPublishedNewsCount();
        //result.Data.TotalPublishedNewsCount = totalPublishedNewsCount.Data;

        //var dailyPublishedNewsCount = await newsCachingService.GetDailyPublishedNewsCount();
        //result.Data.DailyPublishedNewsCount = dailyPublishedNewsCount.Data;

        //var dailyTotalViewsCount = await newsCachingService.GetDailyTotalViewsCount();
        //result.Data.DailyTotalViewsCount = dailyTotalViewsCount.Data;

        //var currentViewerCount = await newsCachingService.GetCurrentViewerCount();
        //result.Data.CurrentViewerCount = currentViewerCount.Data;

        //var dailyCreatedNewsCount = await newsCachingService.GetDailyCreatedNewsCount();
        //result.Data.DailyCreatedNewsCount = dailyCreatedNewsCount.Data;

        //var newsWithTagsCount = await newsCachingService.GetNewsWithTagsCount();
        //result.Data.NewsWithTagsCount = newsWithTagsCount.Data;

        //var newsWithRelatedNewsCount = await newsCachingService.GetNewsWithRelatedNewsCount();
        //result.Data.NewsWithRelatedNewsCount = newsWithRelatedNewsCount.Data;

        var logs = await _systemLogCachingService.GetHomeLogs();
        result.Data.SystemLogs = logs.Data.Select(x => new SystemLog
        {
            CreatedDate = x.CreatedDate,
            UserName = x.UserName,
            ID = x.ID,
            Message = x.Message
        }).ToList();

		//List<long> newsIds = new List<long> { }; // MOCK DATALARIN SİLNİMESİ İÇİN YAZILDI

		//foreach (var newsId in newsIds)
		//{
		//    var socialMediaRecords = await _context.NewsSocialMedias.Where(x => x.NewsID == newsId).ToListAsync();
		//    _context.NewsSocialMedias.RemoveRange(socialMediaRecords);

		//    var versionRecords = await _context.NewsVersions.Where(x => x.NewsID == newsId).ToListAsync();
		//    _context.NewsVersions.RemoveRange(versionRecords);

		//    var versionPhotos = await _context.NewsVersionsPhotos.Where(x => x.NewsID == newsId).ToListAsync();
		//    _context.NewsVersionsPhotos.RemoveRange(versionPhotos);

		//    var photoRecords = await _context.NewsPhotos.Where(x => x.NewsID == newsId).ToListAsync();
		//    _context.NewsPhotos.RemoveRange(photoRecords);

		//    var newsPublishedPhotos = await _context.NewsPublishedPhotos.Where(x => x.NewsID == newsId).ToListAsync();
		//    _context.NewsPublishedPhotos.RemoveRange(newsPublishedPhotos);

		//    var tagRecords = await _context.NewsTags.Where(x => x.NewsID == newsId).ToListAsync();
		//    _context.NewsTags.RemoveRange(tagRecords);

		//    var newsRecords = await _context.News.Where(x => x.ID == newsId).ToListAsync();
		//    _context.News.RemoveRange(newsRecords);  

		//    var newsPublished = await _context.NewsPublished.Where(x => x.NewsID == newsId).ToListAsync();
		//    _context.NewsPublished.RemoveRange(newsPublished);

		//    await _context.SaveChangesAsync();
		//}
	
        return result;
    }




	public async Task<BaseDataTableResponse<List<HomeResponseNewsItem>>> GetNewsPaged(BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<HomeResponseNewsItem>>();

        var query = (from n in _context.News
					 join s in _context.NewsStatus on n.StatusID equals s.ID
					 join c in _context.Categories on n.CategoryID equals c.ID into categories
					 from c in categories.DefaultIfEmpty()
					 join t in _context.NewsTypes on n.NewsTypeID equals t.ID
					 let photo = (from ph in _context.NewsPhotos
								  where ph.NewsID == n.ID &&
										(ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
										 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
								  orderby
									  ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
									  ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 : 2
								  select ph).FirstOrDefault()
                     where n.IsPublished && n.PublishDate.Value.Date == DateTime.Now.Date
					 orderby n.ViewCount descending
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
                         PhotoUrl = photo != null ? photo.PhotoUrl : string.Empty,
                         PublishedDate = n.PublishDate,
                         Tags = n.Tags,
                         IsLockedAuthorized = (n.IsLocked && !(n.LockedBy == CurrentUserID)),
                         StatusId = n.StatusID
                     }).AsNoTracking().Take(20).AsQueryable();

        result.RecordsTotal = await query.CountAsync();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Title.ToUpper().Contains(tableRequest.SearchValue.ToUpper()));
        }

        result.RecordsFiltered = await query.CountAsync();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(f => new HomeResponseNewsItem
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
            RelatedNewsCount = _context.RelatedNews.Count(x => x.NewsID == f.ID),
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

    public async Task<ServiceResult<GetDailyNewsInformationResponse>> GetDailyNewsInformation()
    {
        var result = new ServiceResult<GetDailyNewsInformationResponse>();

        var todaysNews = await _context.News.Where(x => x.CreatedDate.Date == DateTime.Now.Date).ToListAsync();

        var todaysNewsIds = todaysNews.Select(x => x.ID).ToList();

        var taggedNewsCount = await _context.NewsTags
            .Where(nt => todaysNewsIds.Contains((int)nt.NewsID))
            .Select(nt => nt.NewsID)
            .Distinct()
            .CountAsync();

        var relatedNewsCount = await _context.RelatedNews
            .Where(rn => todaysNewsIds.Contains((int)rn.NewsID))
            .Select(rn => rn.NewsID)
            .Distinct()
            .CountAsync();

        var withoutTagNewsCount = todaysNews.Count - taggedNewsCount;
        var withoutRelatedNewsCount = todaysNews.Count - relatedNewsCount;

        result.Data = new GetDailyNewsInformationResponse
        {
            RelatedNewsCount = relatedNewsCount,
            WithoutRelatedNewsCount = withoutRelatedNewsCount,
            TaggedNewsCount = taggedNewsCount,
            WithoutTaggedNewsCount = withoutTagNewsCount
        };

        return result;
    }

    public async Task<ServiceResult<GetNewsDetailsForWidgetResponse>> GetTodayNewsDetails()
    {
        var result = new ServiceResult<GetNewsDetailsForWidgetResponse>();

        var tagCounts = await _context.NewsTags
                                        .GroupBy(ntg => ntg.NewsID)
                                        .Select(g => new { NewsID = g.Key, TagCount = g.Count() })
                                        .ToDictionaryAsync(x => x.NewsID, x => x.TagCount);

        var dailyNewsWithViews = await (from n in _context.News
                                        join ct in _context.Categories on n.CategoryID equals ct.ID
                                        join nt in _context.NewsTypes on n.NewsTypeID equals nt.ID
										let photo = (from ph in _context.NewsPhotos
													 where ph.NewsID == n.ID &&
														   (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
															ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
													 orderby
														 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
														 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 : 2
													 select ph).FirstOrDefault()
										where n.CreatedDate.Date == DateTime.Now.Date && n.IsPublished
                                        select new GetNewsDetailsForWidgetResponse
                                        {
                                            NewsTypeName = nt.Name,
                                            NewsTitle = n.Title ?? n.Headline,
                                            NewsSourceIDs = n.SourceIDs,
                                            NewsID = n.ID,
                                            CategoryName = ct.Name,
                                            TagCount = tagCounts.ContainsKey(n.ID) ? tagCounts[n.ID] : 0,
                                            NewsImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder)
                                        }).FirstOrDefaultAsync();

        if(dailyNewsWithViews != null)
        {
            var dailyNewsSourceNames = await _context.Sources.Where(x => x.ID.ToString().Contains(dailyNewsWithViews.NewsSourceIDs)).Select(x => x.Name).ToListAsync();
            dailyNewsWithViews.NewsSourceName = string.Join(",", dailyNewsSourceNames);
        }

        if (dailyNewsWithViews == null)
        {
            dailyNewsWithViews = await (from n in _context.News
                                        join ct in _context.Categories on n.CategoryID equals ct.ID
                                        join nt in _context.NewsTypes on n.NewsTypeID equals nt.ID
										let photo = (from ph in _context.NewsPhotos
													 where ph.NewsID == n.ID &&
														   (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
															ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
													 orderby
														 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
														 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 : 2
													 select ph).FirstOrDefault()
                                        where n.CreatedDate.Date == DateTime.Now.Date && n.IsPublished
										orderby n.CreatedDate descending 
                                        select new GetNewsDetailsForWidgetResponse
                                        {
                                            NewsTypeName = nt.Name,
                                            NewsTitle = n.Title ?? n.Headline,
                                            NewsSourceIDs = n.SourceIDs,
                                            NewsID = n.ID,
                                            CategoryName = ct.Name,
                                            TagCount = tagCounts.ContainsKey(n.ID) ? tagCounts[n.ID] : 0,
											NewsImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder)
										}).FirstOrDefaultAsync();

            if(dailyNewsWithViews != null)
            {

                var dailyNewsSourceNamesOther = await _context.Sources.Where(x => x.ID.ToString().Contains(dailyNewsWithViews.NewsSourceIDs)).Select(x => x.Name).ToListAsync();
                dailyNewsWithViews.NewsSourceName = string.Join(",", dailyNewsSourceNamesOther);
            }

        }

        result.Data = dailyNewsWithViews;
        return result;
    }

    public async Task<ServiceResult<GetNewsDetailsForWidgetResponse>> GetMostReadedNewsDetails()
    {
        var result = new ServiceResult<GetNewsDetailsForWidgetResponse>();

        //var mostViewedNewsInformations = await (from dv in statisticsContext.NewsViewsDailies
        //                            group dv by dv.NewsID into grouped
        //                            orderby grouped.Sum(x => x.ViewCount) descending
        //                            select new
        //                            {
        //                                NewsID = grouped.Key,
        //                                TotalViews = grouped.Sum(x => x.ViewCount)
        //                            }).FirstOrDefaultAsync();

        //var tagCounts = await _context.NewsTags
        //      .GroupBy(ntg => ntg.NewsID)
        //      .Select(g => new { NewsID = g.Key, TagCount = g.Count() })
        //      .ToDictionaryAsync(x => x.NewsID, x => x.TagCount);

        var mostViewedNews = await (from n in _context.News
                                    join ct in _context.Categories on n.CategoryID equals ct.ID
                                    join nt in _context.NewsTypes on n.NewsTypeID equals nt.ID
                                    let photo = (from ph in _context.NewsPhotos
												 where ph.NewsID == n.ID &&
													   (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
														ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
												 orderby
													 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
													 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 : 2
												 select ph).FirstOrDefault()
									where n.IsPublished
                                    orderby n.ViewCount descending
                                    select new GetNewsDetailsForWidgetResponse
                                    {
                                        NewsTypeName = nt.Name,
                                        NewsTitle = n.Title ?? n.Headline,
                                        NewsSourceIDs = n.SourceIDs,
                                        NewsID = n.ID,
                                        CategoryName = ct.Name,
                                        TagCount =  0,
                                        Impression = n.ViewCount,
										NewsImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder),

										NewsSourceName = string.Empty
                                    }).FirstOrDefaultAsync();

        result.Data = mostViewedNews;
        return result;
    }

    public async Task<ServiceResult<int>> GetUnpublishedNewsCount()
    {
        var result = new ServiceResult<int>();

        var unPublishedNewsCount = await _context.News
            .Where(x => x.StatusID != (int)NewsStatusEnum.Published)
            .CountAsync();

        result.Data = unPublishedNewsCount;
        return result;
    }

    public async Task<ServiceResult<GetNewsTypesResponse>> GetNewsTypeDetails()
    {
        var result = new ServiceResult<GetNewsTypesResponse>();

        var newsTypeDetails = await (from nt in _context.NewsTypes
                                     join n in _context.News on nt.ID equals n.NewsTypeID
                                     group n by new { nt.Name, nt.ID } into grouped
                                     select new NewsTypeItem
                                     {
                                         NewsTypeName = grouped.Key.Name,
                                         NewsTypeID = grouped.Key.ID, 
                                         NewsCount = grouped.Count()
                                     }).ToListAsync();



        result.Data = new GetNewsTypesResponse
        {
            TotalNewsCount = newsTypeDetails.Sum(item => item.NewsCount),
            NewsTypeItems = newsTypeDetails 
        };

        return result;
    }

    public async Task<ServiceResult<GetLastPublishedNewsResponse>> GetLastPublishedNews()
    {
        var result = new ServiceResult<GetLastPublishedNewsResponse>();

        var lastPublishedNews = await (from n in _context.NewsPublished
                                       join nt in _context.NewsTypes on n.NewsTypeID equals nt.ID
									   let photo = (from ph in _context.NewsPhotos
													where ph.NewsID == n.NewsID &&
														  (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
														   ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
													orderby
														ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
														ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 : 2
													select ph).FirstOrDefault()
									   let a = (from au in _context.Authors
												where n.AuthorID == au.ID
												select au).FirstOrDefault()
									   orderby n.PublishedDate descending
                                       select new GetLastPublishedNewsResponse
                                       {
										   NewsID = n.NewsID,
										   AuthorID = a != null ? a.ID : (int?)null,
										   NewsImageURL = photo != null ? photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder) : null,
										   NewsTitle = n.Title ?? n.Headline,
										   NewsSubtitle = n.Subtitle ?? n.Content,
										   PublishDate = n.PublishedDate.ToFormattedDateTime(),
										   AuthorImageURL = a != null ? a.Photo.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos) : string.Empty,
										   AuthorName = a != null ? $"{a.Firstname} {a.Lastname}" : string.Empty
									   }).FirstOrDefaultAsync();

        result.Data = lastPublishedNews;
        return result;
    }

    public async Task<ServiceResult<GetNewsChartDataByCategoriesResponse>> GetNewsChartDataByCategories()
    {
        var result = new ServiceResult<GetNewsChartDataByCategoriesResponse>
        {
            Data = new GetNewsChartDataByCategoriesResponse
            {
                Categories = new List<string>(),
                NewsCountsByCategory = new List<int>()
            }
        };

		var newsChartData = await (from n in _context.News
								   join c in _context.Categories on n.CategoryID equals c.ID
								   group n by c.Name into grouped
								   orderby grouped.Count() descending
								   select new
								   {
									   CategoryName = grouped.Key,
									   NewsCount = grouped.Count()
								   }).Take(20).ToListAsync();

		result.Data.Categories = newsChartData.Select(x => x.CategoryName).ToList();
        result.Data.NewsCountsByCategory = newsChartData.Select(x => x.NewsCount).ToList();
        return result;
    }

    public async Task<ServiceResult<GetNewsChartDataByDailyNewsResponse>> GetNewsChartDataByDailyNews()
    {
        var result = new ServiceResult<GetNewsChartDataByDailyNewsResponse> 
        { 
            Data = new GetNewsChartDataByDailyNewsResponse 
            { 
                NewsCounts = new List<int>(), 
                NewsCreateDates = new List<string>() 
            }
        };

        var newsChartData = await (from n in _context.News
                                   where n.CreatedDate >= DateTime.Now.AddDays(-14)
                                   group n by n.CreatedDate.Date into grouped
                                   select new
                                   {
                                       Date = grouped.Key,
                                       NewsCount = grouped.Count()
                                   }).ToListAsync();

        result.Data.NewsCounts = newsChartData.Select(x => x.NewsCount).ToList();
        result.Data.NewsCreateDates = newsChartData.Select(x => x.Date.ToString("dd.MM.yyyy")).ToList();
        return result;
    }
}