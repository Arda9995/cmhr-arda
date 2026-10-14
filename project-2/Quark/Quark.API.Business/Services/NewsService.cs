using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.NewsResponses;
using Quark.API.Caching.Models.Responses.NewsCachingResponses;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.AuthorCachingResponses;
using Quark.Business.Caching.Models.Responses.NewsCachingResponses;
using Quark.Business.Caching.Models.Responses.PageCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Core.Extensions;
using Quark.Core.Extentions;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.Survey;
using Quark.Resources;
using System.Globalization;
using RelatedNews = Quark.API.Business.Models.Responses.NewsResponses.RelatedNews;

namespace Quark.API.Business.Services
{
    public class NewsService(
        IHttpContextAccessor httpContext,
        QuarkDataContext context,
        IStringLocalizer<CommonResource> localizer,
        Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys,
        ICacheManager cache,
        INewsCachingService _newsCachingService,
        ITagCachingService _tagCachingService,
        IAuthorCachingService _authorCachingService,
        IBlockCachingService _blockCachingService,
        IServiceProvider _serviceProvider,
        ICategoryCachingService _categoryCachingService)
        : BaseService(httpContext, context, localizer, appSettingsKeys, cache), INewsService
    {
        public async Task<ServiceResult> RemoveNewsByIDList(long[] IDList)
        {
            var result = new ServiceResult();

            // Transaction başlatıyoruz
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var id in IDList)
                {
                    // News
                    var news = await _context.News.FirstOrDefaultAsync(x => x.ID == id);
                    if (news != null)
                    {
                        _context.News.Remove(news);
                    }

                    // NewsVersions
                    var newsVersions = await _context.NewsVersions.Where(x => x.NewsID == id).ToListAsync();
                    if (newsVersions.Any())
                    {
                        _context.NewsVersions.RemoveRange(newsVersions);
                    }

                    // NewsPublished
                    var newsPublished = await _context.NewsPublished.FirstOrDefaultAsync(x => x.NewsID == id);
                    if (newsPublished != null)
                    {
                        _context.NewsPublished.Remove(newsPublished);
                    }

                    // NewsSocialMedias
                    var newsSocialMedias = await _context.NewsSocialMedias.Where(x => x.NewsID == id).ToListAsync();
                    if (newsSocialMedias.Any())
                    {
                        _context.NewsSocialMedias.RemoveRange(newsSocialMedias);
                    }

                    // NewsPhotos
                    var newsPhotos = await _context.NewsPublishedPhotos.Where(x => x.NewsID == id).ToListAsync();
                    if (newsPhotos.Any())
                    {
                        _context.NewsPublishedPhotos.RemoveRange(newsPhotos);
                    }

                    // NewsVersionPhotos
                    var newsVersionPhotos = await _context.NewsVersionsPhotos.Where(x => x.NewsID == id).ToListAsync();
                    if (newsVersionPhotos.Any())
                    {
                        _context.NewsVersionsPhotos.RemoveRange(newsVersionPhotos);
                    }

                    // NewsPublishedPhotos
                    var newsPublishedPhotos =
                        await _context.NewsPublishedPhotos.Where(x => x.NewsID == id).ToListAsync();
                    if (newsPublishedPhotos.Any())
                    {
                        _context.NewsPublishedPhotos.RemoveRange(newsPublishedPhotos);
                    }

                    // NewsPhotoGalleryItems
                    var newsPhotoGalleryItems =
                        await _context.NewsPhotoGalleryItems.Where(x => x.NewsID == id).ToListAsync();
                    if (newsPhotoGalleryItems.Any())
                    {
                        _context.NewsPhotoGalleryItems.RemoveRange(newsPhotoGalleryItems);
                    }

                    // NewsPublishedPhotoGalleryItems
                    var newsPublishedPhotoGalleryItems = await _context.NewsPublishedPhotoGalleryItems
                        .Where(x => x.NewsID == id).ToListAsync();
                    if (newsPublishedPhotoGalleryItems.Any())
                    {
                        _context.NewsPublishedPhotoGalleryItems.RemoveRange(newsPublishedPhotoGalleryItems);
                    }

                    // NewsVersionPhotoGalleryItems
                    var newsVersionPhotoGalleryItems = await _context.NewsVersionsPhotoGalleryItems
                        .Where(x => x.NewsID == id).ToListAsync();
                    if (newsVersionPhotoGalleryItems.Any())
                    {
                        _context.NewsVersionsPhotoGalleryItems.RemoveRange(newsVersionPhotoGalleryItems);
                    }

                    // NewsTags
                    var newsTags = await _context.NewsTags.Where(x => x.NewsID == id).ToListAsync();
                    if (newsTags.Any())
                    {
                        _context.NewsTags.RemoveRange(newsTags);
                    }

                    // RelatedNews (Hem NewsID hem RelatedNewsID olanları sil)
                    var relatedNews = await _context.RelatedNews.Where(x => x.NewsID == id || x.RelatedNewsID == id)
                        .ToListAsync();
                    if (relatedNews.Any())
                    {
                        _context.RelatedNews.RemoveRange(relatedNews);
                    }

                    var secondaryCategories = await _context.NewsSecondaryCategories
                        .Where(x => x.NewsID == id).ToListAsync();
                    if (secondaryCategories.Any())
                    {
                        _context.NewsSecondaryCategories.RemoveRange(secondaryCategories);
                    }

                    var sources = await _context.NewsSources
                        .Where(x => x.NewsID == id).ToListAsync();

                    if (sources.Any())
                    {
                        _context.NewsSources.RemoveRange(sources);
                    }
                }

                // Değişiklikleri kaydet
                await _context.SaveChangesAsync();

                // Transaction'u commit et
                await transaction.CommitAsync();
                result.Message = "Silme işlemi başarıyla tamamlandı.";
            }
            catch (Exception ex)
            {
                // Hata durumunda rollback yap
                await transaction.RollbackAsync();
                result.Message = $"Silme işlemi sırasında bir hata oluştu: {ex.Message}";
            }

            return result;
        }

        public async Task<ServiceResult<GetNewsDetailResponse>> GetNewsDetail(int ID)
        {
            var result = new ServiceResult<GetNewsDetailResponse>();

            var news = await _newsCachingService.GetNewsDetailByID(ID);

            if (!news.IsSuccess)
            {
                var newsDetail = await GetNewsDetailService(ID);
                if (!newsDetail.IsSuccess)
                {
                    result.SetError(newsDetail.Message);
                    return result;
                }

                result.Data = newsDetail.Data;
            }
            else
            {
                result.Data = new GetNewsDetailResponse()
                {
                    ID = news.Data.ID,
                    NewsSlug = news.Data.NewsSlug,
                    NewsTypeID = news.Data.NewsTypeID,
                    Title = news.Data.Title,
                    UpHeadline = news.Data.UpHeadline,
                    Headline = news.Data.Headline,
                    SubTitle = news.Data.SubTitle,
                    Content = news.Data.Content,
                    CoverPhoto = news.Data.CoverPhoto,
                    CreatedDate = news.Data.CreatedDate,
                    UpdateDate = news.Data.UpdateDate,
                    UpdateDateForMetaTag = news.Data.UpdateDateForMetaTag,
                    PublishDate = news.Data.PublishDate,
                    PublishDateForMetaTag = news.Data.PublishDateForMetaTag,
                    SecondaryCategoryIDList = news.Data.SecondaryCategoryIDList,
                    SecondaryCategoryNameList = news.Data.SecondaryCategoryNameList,
                    Subheading = news.Data.Subheading,
                    UpSubheading = news.Data.UpSubheading,
                    UpSubtitle = news.Data.UpSubtitle,
                    RelatedNewsList = news.Data.RelatedNewsList.Select(x => new RelatedNews
                    {
                        ID = x.ID,
                        Title = x.Title,
                        Subtitle = x.Subtitle,
                        Slug = x.Slug,
                        ImageURL = x.ImageURL
                    }).ToList(),
                    CategoryNewsList = news.Data.CategoryNewsList.Select(x => new CategoryNews
                    {
                        ID = x.ID,
                        Headline = x.Headline,
                        UpHeadline = x.UpHeadline,
                        Slug = x.Slug,
                        SlugWithOutClientURL = x.SlugWithOutClientURL,
                        UpSubtitle = x.UpSubtitle,
                        ImageURL = x.ImageURL,
                        PublishDate = x.PublishDate,
                        PressNo = x.PressNo
                    }).ToList(),
                    PhotoGalleryItems = news.Data.PhotoGalleryItems.Select(x => new PhotoGalleryItem
                    {
                        ImageURL = x.ImageURL,
                        Content = x.Content
                    }).ToList(),
                    TagList = news.Data.TagList.Select(x => new TagList
                    {
                        ID = x.ID,
                        Text = x.Text,
                        Slug = x.Slug
                    }).ToList(),
                    Tags = news.Data.Tags,
                    SourceList = news.Data.SourceList.Select(x => new SourceList
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Slug = x.Slug
                    }).ToList(),
                    Sources = news.Data.Sources,
                    ColumnArticleDetails = new ColumnArticleDetails
                    {
                        AuthorPhoto = news.Data.CachingNewsDetailColumnArticleDetails.AuthorPhoto,
                        AuthorFullName = news.Data.CachingNewsDetailColumnArticleDetails.AuthorFullName,
                        AuthorSlug = news.Data.CachingNewsDetailColumnArticleDetails.AuthorSlug,
                        AuthorSocialX = news.Data.CachingNewsDetailColumnArticleDetails.AuthorSocialX,
                        AuthorSocialInstagram = news.Data.CachingNewsDetailColumnArticleDetails.AuthorSocialInstagram,
                        AuthorSocialFacebook = news.Data.CachingNewsDetailColumnArticleDetails.AuthorSocialFacebook,
                        AuthorSocialLinkedIn = news.Data.CachingNewsDetailColumnArticleDetails.AuthorSocialLinkedIn,
                        AuthorSocialMail = news.Data.CachingNewsDetailColumnArticleDetails.AuthorSocialMail,
                    },
                    SurveyElements = news.Data.CachingNewsDetailSurveyElements == null
                        ? null
                        : new SurveyElements
                        {
                            SurveyQuestion = news.Data.CachingNewsDetailSurveyElements.SurveyQuestion,
                            JoinedUserCount = news.Data.CachingNewsDetailSurveyElements.JoinedUserCount,
                            RemainingTime = news.Data.CachingNewsDetailSurveyElements.RemainingTime,
                            SurveySections = news.Data.CachingNewsDetailSurveyElements.SurveySections.Select(x =>
                                new SurveySection
                                {
                                    OptionID = x.OptionID,
                                    SurveyID = x.SurveyID,
                                    Option = x.Option,
                                    Percent = x.Percent
                                }).ToList(),
                            CategoryNews = news.Data.CachingNewsDetailSurveyElements.CategoryNews.Select(x =>
                                new CategoryNews
                                {
                                    ID = x.ID,
                                    Headline = x.Headline,
                                    UpHeadline = x.UpHeadline,
                                    Slug = x.Slug,
                                    SlugWithOutClientURL = x.SlugWithOutClientURL,
                                    UpSubtitle = x.UpSubtitle,
                                    ImageURL = x.ImageURL,
                                    PublishDate = x.PublishDate,
                                    PressNo = x.PressNo
                                }).ToList(),
                            SurveyNewsThird = news.Data.CachingNewsDetailSurveyElements.SurveyNewsThird.Select(x =>
                                new SurveyNews
                                {
                                    ID = x.ID,
                                    Title = x.Title,
                                    Question = x.Question,
                                    Slug = x.Slug,
                                    ImageURL = x.ImageURL,
                                    RemainingTime = x.RemainingTime,
                                    JoinedUserCount = x.JoinedUserCount
                                }).ToList(),
                            SurveyNewsQuad = news.Data.CachingNewsDetailSurveyElements.SurveyNewsQuad.Select(x =>
                                new SurveyNews
                                {
                                    ID = x.ID,
                                    Title = x.Title,
                                    Question = x.Question,
                                    Slug = x.Slug,
                                    ImageURL = x.ImageURL,
                                    RemainingTime = x.RemainingTime,
                                    JoinedUserCount = x.JoinedUserCount
                                }).ToList()
                        },
                    SourceIDs = news.Data.SourceIDs,
                    AuthorID = news.Data.AuthorID,
                    AuthorName = news.Data.AuthorName,
                    AuthorSlug = news.Data.AuthorSlug,
                    CategoryName = news.Data.CategoryName,
                    CategoryBreadcrumb = news.Data.CategoryBreadcrumb,
                    CategorySlug = news.Data.CategorySlug,
                    BlockQuote = news.Data.BlockQuote,
                    CategoryID = news.Data.CategoryID,
                    PressNo = news.Data.PressNo,
                    BoxCount = news.Data.BoxCount,
                    VideoPlayerID = news.Data.VideoPlayerID,
                    Video = news.Data.Video,
                    ScrollNewsIDs = news.Data.ScrollNewsIDs,
                    NewsDetailNextNewsIDs = news.Data.NewsDetailNextNewsIDs,
                    SecondaryCategoryMap = news.Data.SecondaryCategoryMap,
                    SeoDescription = news.Data.SeoDescription,
                    SeoTitle = news.Data.SeoTitle
                };

                if (news.Data.NewsTypeID == (int)NewsTypesEnum.ColumnArticles)
                {
                    var authorsLastColumnArticles = await _newsCachingService.GetAuthorLastNews(news.Data.AuthorID ?? 0);

                    result.Data.ColumnArticleDetails.AuthorsLastColumnArticles = authorsLastColumnArticles.Data.Select(x => new AuthorsLastColumnArticles
                    {
                        Headline = x.Headline,
                        Slug = x.Slug,
                        UpSubtitle = x.UpSubtitle,
                        NewsID = x.NewsID,
                        PublishDate = x.PublishDate
                    }).ToList();
                }
            }

            #region Scrool News

            List<int> randomNewsIds;

            if (result.Data.NewsTypeID == (int)NewsTypesEnum.PhotoGallery || result.Data.NewsTypeID == (int)NewsTypesEnum.News)
            {
                var randomNewsIdsByElastic = await _newsCachingService.GetNewsIdsByNewsType(result.Data.NewsTypeID, 10);
                if (!randomNewsIdsByElastic.IsSuccess || randomNewsIdsByElastic.Data == null || !randomNewsIdsByElastic.Data.Any())
                {
                    randomNewsIds = await GetRandomRelatedNewsIds(result.Data.ID, result.Data.NewsTypeID,
                        result.Data.CategoryID,
                        result.Data.SecondaryCategoryMap);
                }
                else
                {
                    randomNewsIds = randomNewsIdsByElastic.Data;
                }
            }
            else if (result.Data.NewsTypeID == (int)NewsTypesEnum.ZodiacSigns)
            {
                var zodiacSignNews = await GetZodiacSignNewsListForIndex();
                if (!zodiacSignNews.IsSuccess || zodiacSignNews.Data.ZodiacSignItems == null ||
                    !zodiacSignNews.Data.ZodiacSignItems.Any())
                {
                    randomNewsIds = await GetRandomRelatedNewsIds(result.Data.ID, result.Data.NewsTypeID,
                        result.Data.CategoryID,
                        result.Data.SecondaryCategoryMap);
                }
                else
                {
                    randomNewsIds = zodiacSignNews.Data.ZodiacSignItems.Select(x => (int)x.NewsID).ToList();
                }
            }
            else if (result.Data.NewsTypeID == (int)NewsTypesEnum.ColumnArticles)
            {
                var columnArticlesNews = await _newsCachingService.GetAuthorSideColumnArticles();
                if (!columnArticlesNews.IsSuccess || columnArticlesNews.Data == null ||
                    !columnArticlesNews.Data.Any())
                {
                    randomNewsIds = await GetRandomRelatedNewsIds(result.Data.ID, result.Data.NewsTypeID,
                        result.Data.CategoryID,
                        result.Data.SecondaryCategoryMap);
                }
                else
                {
                    randomNewsIds = columnArticlesNews.Data.Select(x => (int)x.NewsID).ToList();
                }
            }
            else
            {
                var randomNewsIdsByElastic = await _newsCachingService.GetNewsIdsByNewsType(result.Data.NewsTypeID, 7);
                if (!randomNewsIdsByElastic.IsSuccess || randomNewsIdsByElastic.Data == null || !randomNewsIdsByElastic.Data.Any())
                {
                    randomNewsIds = await GetRandomRelatedNewsIds(result.Data.ID, result.Data.NewsTypeID,
                        result.Data.CategoryID,
                        result.Data.SecondaryCategoryMap);
                }
                else
                {
                    randomNewsIds = randomNewsIdsByElastic.Data;
                }
            }


            if (randomNewsIds != null)
            {
                result.Data.ScrollNewsIDs = string.Join(",", randomNewsIds);
                result.Data.NewsDetailNextNewsIDs = randomNewsIds;
            }

            #endregion

            return result;
        }

        private async Task<List<int>> GetRandomRelatedNewsIds(int currentNewsId, int newsTypeId, int categoryId,
            string secondaryCategoryMap)
        {
            var searchNews = await (from np in _context.NewsPublished
                                    where np.NewsTypeID == newsTypeId && np.PublishedDate > DateTime.Now.AddDays(-2)
                                    select new GetNewsPublishedCacheResponse()
                                    {
                                        ID = (int)np.NewsID,
                                        NewsTypeID = np.NewsTypeID
                                    }).ToListAsync();

            var random = new Random();
            var shuffledIds = searchNews
                .Where(x => x.ID != currentNewsId && x.NewsTypeID == newsTypeId)
                .Select(x => x.ID)
                .OrderBy(x => random.Next())
                .Take(10)
                .ToList();

            return shuffledIds;
        }

        public async Task<ServiceResult<int>> GetNewsType(int newsID)
        {
            var result = new ServiceResult<int>();

            //var publishedNewsData = await _newsCachingService.GetPublishedNewsUrls();
            var publishedNewsData = await _context.NewsPublished
                    .AsNoTracking()
                    .Where(x => x.NewsID == newsID)
                    .Select(x => x.NewsTypeID)
                    .FirstOrDefaultAsync();

            result.Data = publishedNewsData;
            return result;
        }

        public async Task<ServiceResult<int>> CheckCategoryTypeBySlug(string slug)
        {
            var result = new ServiceResult<int>();
            var categoriesFromRedis = await _categoryCachingService.GetAllCategories();

            int categoryID = 0;
            if (categoriesFromRedis.IsSuccess)
            {
                var category = categoriesFromRedis.Data.Where(x => x.Url == slug).Select(x => x.ID).FirstOrDefault();

                categoryID = category;

                if (category == 0)
                {
                    result.SetError("Kategori bulunamadı.");
                    return result;
                }

                if (category != null)
                {
                    result.Data = category;
                    return result;
                }
            }

            result.Data = categoryID;
            return result;
        }

        public async Task<ServiceResult<GetNewsListResponse>> GetNewsByCategorySlug(int categoryID)
        {
            var result = new ServiceResult<GetNewsListResponse>
            {
                Data = new GetNewsListResponse
                {
                    SurveyNewsTwo = new List<SurveyNews>(),
                    SurveyNewsSix = new List<SurveyNews>(),
                    SurveyNewsOthers = new List<SurveyNews>(),
                    CategoryTwo = new List<CategoryNews>(),
                    CategorySix = new List<CategoryNews>(),
                    CategoryTen = new List<CategoryNews>(),
                    CategoryOthers = new List<CategoryNews>(),
                    ZodiacSignNews = new List<CategoryNews>(),
                    ZodiacSignNewsForSlider = new List<ZodiacSignNewsForSlider>(),
                }
            };

            var cachingModel = new GetNewsListResponse();
            var getCachingResponse = await _newsCachingService.GetCachingNewsByCategorySlug(categoryID);

            if (!getCachingResponse.IsSuccess)
            {
                var createResponse = await CreateGetNewsByCategorySlug(categoryID);
                result.Data = createResponse.Data;
                await _newsCachingService.RefreshNewsByCategorySlug(categoryID);
                return result;
            }

            cachingModel = MapToResponse(getCachingResponse.Data);
            result.Data = cachingModel;

            return result;
        }

        public async Task<ServiceResult<List<GetNewsListSearchResponse>>> GetNewsListBySearchKey(string search)
        {
            var result = new ServiceResult<List<GetNewsListSearchResponse>>();

            var newsCaching = await _newsCachingService.GetNewsPublishedSearch(search, 5);

            if (!newsCaching.IsSuccess)
            {
                result.SetError("Arama sonuçları alınamadı.");
                return result;
            }

            var newsList = newsCaching.Data.Select(x => new GetNewsListSearchResponse
            {
                UpSubtitle = x.UpSubtitle,
                Headline = x.Headline ?? x.Title,
                Slug = x.Slug,
                ImageURL = x.CoverPhoto
            }).ToList();

            result.Data = newsList;
            return result;
        }

        public async Task<ServiceResult<GetNewsListSearchedResponse>> GetNewsListBySearchedKey(string search)
        {
            var result = new ServiceResult<GetNewsListSearchedResponse>();
            var searchItemCount = 12;

            var searchedNews = await _newsCachingService.GetNewsPublishedSearch(search);

            var totalCount = searchedNews.Data.Count();

            var newsList = searchedNews.Data.Select(x => new GetNewsListSearchResponse
            {
                ID = x.ID,
                UpSubtitle = x.UpSubtitle.IsNullOrEmtpy() ? x.SubTitle : x.UpSubtitle,
                Headline = x.Headline.IsNullOrEmtpy() ? x.Title : x.Headline,
                PublishDate = x.PublishDate.ToFormattedDate(),
                Slug = x.Slug,
                ImageURL = x.CoverPhoto
            }).Take(searchItemCount).ToList();

            var blockList = new List<string>()
            {
                "Anasayfa-EnCokArananlarTab1",
                "Anasayfa-EnCokArananlarTab2",
                "Anasayfa-EnCokArananlarTab3",
                "Anasayfa-EnCokArananlarTab4"
            };

            var cacheBlockNews = await _blockCachingService.GetBlockNews(blockList);

            var isHaveMoreItems = totalCount > searchItemCount;

            var searchedResponse = new GetNewsListSearchedResponse
            {
                TotalNewsCount = totalCount,
                NewsListSix = newsList.Take(6).ToList(),
                NewsListOthers = newsList.Skip(6).ToList(),
                SearchKey = search,
                MostSearchedNews = cacheBlockNews.Data,
                IsHaveMoreItems = isHaveMoreItems,
            };

            result.Data = searchedResponse;
            return result;
        }

        public async Task<ServiceResult<List<PageNewsCachingResponse>>> GetMostSearchedNewsBlock()
        {
            var result = new ServiceResult<List<PageNewsCachingResponse>>();

            result.Data = await (from blck in _context.Blocks
                                 join blcks in _context.BlockItems on blck.ID equals blcks.BlockID
                                 join np in _context.NewsPublished on blcks.NewsID equals np.NewsID
                                 let photo = (from ph in _context.NewsPublishedPhotos
                                              where ph.NewsID == np.NewsID &&
                                                    (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                     ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                              orderby
                                                  ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                      ? 0
                                                      : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                          ? 1
                                                          : 2
                                              select ph).FirstOrDefault()
                                 where blck.Key == "Anasayfa-EnCokArananlarTab1" ||
                                       blck.Key == "Anasayfa-EnCokArananlarTab2" ||
                                       blck.Key == "Anasayfa-EnCokArananlarTab3" ||
                                       blck.Key == "Anasayfa-EnCokArananlarTab4"
                                 select new PageNewsCachingResponse
                                 {
                                     BlockName = blck.Name,
                                     BlockTitle = blck.Title,
                                     NewsPhoto = _context.NewsPublishedPhotos
                                         .Where(p => p.NewsID == np.NewsID && p.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                         .FirstOrDefault().PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                             _appSettingsKeys.CDN_MediaGalleryFolder),
                                     NewsHeadline = !string.IsNullOrEmpty(np.Headline) ? np.Headline
                                         : !string.IsNullOrEmpty(np.Title) ? np.Title
                                         : null,
                                     BlockKey = blck.Key,
                                     NewsSlug = np.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                 }).ToListAsync();
            return result;
        }

        public async Task<ServiceResult<LoadMoreNewsResponse>> GetMoreNewsByCategoryId(int categoryId, int page,
            int pageSize)
        {
            var result = new ServiceResult<LoadMoreNewsResponse>
            {
                Data = new LoadMoreNewsResponse()
            };

            // Calculate skip amount based on page and page size
            int skip = page * pageSize;

            // Get more news items from the category
            var categoryNews = await (from np in _context.NewsPublished
                                      let photo = (from ph in _context.NewsPublishedPhotos
                                                   where ph.NewsID == np.NewsID &&
                                                         (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                          ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                                   orderby
                                                       ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                           ? 0
                                                           : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                                             !string.IsNullOrEmpty(ph.PhotoUrl)
                                                               ? 1
                                                               : 2
                                                   select ph).FirstOrDefault()
                                      where np.CategoryID == categoryId
                                      orderby np.PublishedDate descending
                                      select new CategoryNews
                                      {
                                          ID = (int)np.NewsID,
                                          Headline = !string.IsNullOrEmpty(np.Headline) ? np.Headline
                                              : !string.IsNullOrEmpty(np.Title) ? np.Title
                                              : null,
                                          UpSubtitle = !string.IsNullOrEmpty(np.UpSubtitle) ? np.UpSubtitle
                                              : !string.IsNullOrEmpty(np.Subtitle) ? np.Subtitle
                                              : np.Content.ExtractTextFromHtml(),
                                          Slug = np.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                          ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                              _appSettingsKeys.CDN_MediaGalleryFolder),
                                      })
                .Skip(skip)
                .Take(pageSize + 1) // Get one extra to check if there are more items
                .ToListAsync();

            // If we got more items than the page size, there are more items to load
            bool hasMoreItems = categoryNews.Count > pageSize;

            // Remove the extra item we used to check for more items
            if (hasMoreItems)
            {
                categoryNews.RemoveAt(categoryNews.Count - 1);
            }

            result.Data.Items = categoryNews;
            result.Data.HasMoreItems = hasMoreItems;

            return result;
        }

        public (DateTime? startDate, DateTime? endDate) ParseDateRange(string dateRange)
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

        public async Task<ServiceResult<LoadMoreSearchResponse>> GetMoreNewsBySearchKey(string search, int page,
            string dateRange)
        {
            var result = new ServiceResult<LoadMoreSearchResponse>
            {
                Data = new LoadMoreSearchResponse()
            };

            try
            {
                // Calculate skip amount based on page and page size
                var pageSize = 8;
                int skip = page * pageSize;
                var (startDate, endDate) = ParseDateRange(dateRange);

                // Build base query without ordering first
                var searchedNews = await _newsCachingService.GetNewsPublishedSearch(search);
                var query = searchedNews.Data;

                // Apply date filters
                if (startDate.HasValue)
                {
                    query = query.Where(np => np.PublishDate.Date >= startDate.Value.Date).ToList();
                }

                if (endDate.HasValue)
                {
                    query = query.Where(np => np.PublishDate.Date <= endDate.Value.Date).ToList();
                }

                // Get total count for hasMoreItems check

                var totalCount = query.Count();


                // Apply pagination
                var searchResults = query
                    .Skip(skip)
                    .Take(pageSize)
                    .Select(np => new GetNewsListSearchResponse
                    {
                        ID = (int)np.ID,
                        UpSubtitle = np.SubTitle,
                        Headline = np.Headline,
                        Slug = np.Slug,
                        ImageURL = np.CoverPhoto,
                        PublishDate = np.PublishDate.ToFormattedDate()
                    }).ToList();

                // Check if there are more items
                bool hasMoreItems = skip + pageSize < totalCount;

                result.Data.TotalCount = totalCount;
                result.Data.Items = searchResults;
                result.Data.HasMoreItems = hasMoreItems;
            }
            catch (Exception ex)
            {
                result.SetError(ex.Message);
            }

            return result;
        }

        public async Task<ServiceResult<GetNewsListResponse>> GetNewsBySourceSlug(string sourceSlug)
        {
            var result = new ServiceResult<GetNewsListResponse>
            {
                Data = new GetNewsListResponse
                {
                    SurveyNewsTwo = new List<SurveyNews>(),
                    SurveyNewsSix = new List<SurveyNews>(),
                    SurveyNewsOthers = new List<SurveyNews>(),
                    CategoryTwo = new List<CategoryNews>(),
                    CategorySix = new List<CategoryNews>(),
                    CategoryTen = new List<CategoryNews>(),
                    CategoryOthers = new List<CategoryNews>(),
                    ZodiacSignNews = new List<CategoryNews>(),
                    ZodiacSignNewsForSlider = new List<ZodiacSignNewsForSlider>(),
                }
            };

            var cachingModel = new GetNewsListResponse();
            var getCachingResponse = await _newsCachingService.GetCachingNewsBySourceSlug(sourceSlug);

            if (!getCachingResponse.IsSuccess)
            {
                var createResponse = await CreateGetNewsBySourceSlug(sourceSlug);
                if (!createResponse.IsSuccess)
                {
                    result.SetError(createResponse.Message);
                    return result;
                }
                result.Data = createResponse.Data;
                await _newsCachingService.RefreshNewsBySourceSlug(sourceSlug);
                return result;
            }

            cachingModel = MapToResponse(getCachingResponse.Data);
            result.Data = cachingModel;

            return result;
        }

        public async Task<ServiceResult<GetNewsListResponse>> GetNewsByTagSlug(string tagSlug)
        {
            var result = new ServiceResult<GetNewsListResponse>
            {
                Data = new GetNewsListResponse
                {
                    SurveyNewsTwo = new List<SurveyNews>(),
                    SurveyNewsSix = new List<SurveyNews>(),
                    SurveyNewsOthers = new List<SurveyNews>(),
                    CategoryTwo = new List<CategoryNews>(),
                    CategorySix = new List<CategoryNews>(),
                    CategoryTen = new List<CategoryNews>(),
                    CategoryOthers = new List<CategoryNews>(),
                    ZodiacSignNews = new List<CategoryNews>(),
                    ZodiacSignNewsForSlider = new List<ZodiacSignNewsForSlider>(),
                }
            };

            var cachingModel = new GetNewsListResponse();
            var getCachingResponse = await _newsCachingService.GetCachingNewsByTagSlug(tagSlug);

            if (!getCachingResponse.IsSuccess)
            {
                var createResponse = await CreateGetNewsByTagSlug(tagSlug);
                if (!createResponse.IsSuccess)
                {
                    result.SetError(createResponse.Message);
                    return result;
                }
                result.Data = createResponse.Data;
                await _newsCachingService.RefreshNewsByTagSlug(tagSlug);
                return result;
            }

            cachingModel = MapToResponse(getCachingResponse.Data);
            result.Data = cachingModel;

            return result;
        }

        public async Task<ServiceResult<LoadMoreNewsResponse>> GetMoreNewsBySourceSlug(string sourceSlug, int page,
            int pageSize)
        {
            var result = new ServiceResult<LoadMoreNewsResponse>
            {
                Data = new LoadMoreNewsResponse()
            };

            var source = await _context.Sources
                .Where(s => s.Url.Contains(sourceSlug))
                .Select(s => new { s.ID })
                .FirstOrDefaultAsync();

            if (source == null)
            {
                result.Data.Items = new List<CategoryNews>();
                result.Data.HasMoreItems = false;
                return result;
            }

            int skip = page * pageSize;

            var sourceIdPattern = $"%{source.ID},%";
            var sourceIdPatternEnd = $"%,{source.ID}%";
            var sourceIdSingle = $"{source.ID}";

            var newsItems = await _context.NewsPublished
                .Where(np => np.SourceIDs.Contains(sourceIdPattern) ||
                             np.SourceIDs.Contains(sourceIdPatternEnd) ||
                             np.SourceIDs == sourceIdSingle)
                .OrderByDescending(np => np.PublishedDate)
                .Skip(skip)
                .Take(pageSize + 1)
                .Select(np => new
                {
                    NewsPublished = np,
                    PhotoUrl = _context.NewsPublishedPhotos
                        .Where(ph => ph.NewsID == np.NewsID &&
                                     (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                      ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto))
                        .OrderBy(ph =>
                            ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                ? 0
                                : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                  !string.IsNullOrEmpty(ph.PhotoUrl)
                                    ? 1
                                    : 2)
                        .Select(ph => ph.PhotoUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var categoryNews = newsItems
                .Where(x => x.NewsPublished.SourceIDs?.Split(',')
                    .Select(s => !string.IsNullOrEmpty(s) ? int.Parse(s) : 0)
                    .Contains(source.ID) ?? false)
                .Select(x => new CategoryNews
                {
                    ID = (int)x.NewsPublished.NewsID,
                    Headline = !string.IsNullOrEmpty(x.NewsPublished.Headline) ? x.NewsPublished.Headline
                        : !string.IsNullOrEmpty(x.NewsPublished.Title) ? x.NewsPublished.Title
                        : null,
                    UpSubtitle = !string.IsNullOrEmpty(x.NewsPublished.UpSubtitle) ? x.NewsPublished.UpSubtitle
                        : !string.IsNullOrEmpty(x.NewsPublished.Subtitle) ? x.NewsPublished.Subtitle
                        : x.NewsPublished.Content.ExtractTextFromHtml(),
                    Slug = x.NewsPublished.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                    SlugWithOutClientURL = x.NewsPublished.Slug,
                    ImageURL = x.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                        _appSettingsKeys.CDN_MediaGalleryFolder),
                    PublishDate = x.NewsPublished.PublishedDate.ToFormattedDate()
                })
                .ToList();

            bool hasMoreItems = categoryNews.Count > pageSize;

            if (hasMoreItems && categoryNews.Count > pageSize)
            {
                categoryNews = categoryNews.Take(pageSize).ToList();
            }

            result.Data.Items = categoryNews;
            result.Data.HasMoreItems = hasMoreItems;

            return result;
        }

        public async Task<ServiceResult<LoadMoreNewsResponse>> GetMoreNewsByTagSlug(string tagSlug, int page,
            int pageSize)
        {
            var result = new ServiceResult<LoadMoreNewsResponse>
            {
                Data = new LoadMoreNewsResponse()
            };

            int skip = page * pageSize;

            var tag = await _context.Tags
                .Where(t => t.Url.Contains(tagSlug))
                .Select(t => new { t.ID })
                .FirstOrDefaultAsync();

            if (tag == null)
            {
                result.Data.Items = new List<CategoryNews>();
                result.Data.HasMoreItems = false;
                return result;
            }

            var newsWithTag = await _context.NewsTags
                .Where(nt => nt.TagID == tag.ID)
                .Select(nt => nt.NewsID)
                .Distinct()
                .ToListAsync();

            var newsItems = await _context.NewsPublished
                .Where(np => newsWithTag.Contains(np.NewsID))
                .OrderByDescending(np => np.PublishedDate)
                .Skip(skip)
                .Take(pageSize + 1)
                .Select(np => new
                {
                    NewsPublished = np,
                    PhotoUrl = _context.NewsPublishedPhotos
                        .Where(ph => ph.NewsID == np.NewsID &&
                                     (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                      ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto))
                        .OrderBy(ph =>
                            ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                ? 0
                                : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                  !string.IsNullOrEmpty(ph.PhotoUrl)
                                    ? 1
                                    : 2)
                        .Select(ph => ph.PhotoUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var categoryNews = newsItems
                .Select(x => new CategoryNews
                {
                    ID = (int)x.NewsPublished.NewsID,
                    Headline = !string.IsNullOrEmpty(x.NewsPublished.Headline) ? x.NewsPublished.Headline
                        : !string.IsNullOrEmpty(x.NewsPublished.Title) ? x.NewsPublished.Title
                        : null,
                    UpSubtitle = !string.IsNullOrEmpty(x.NewsPublished.UpSubtitle) ? x.NewsPublished.UpSubtitle
                        : !string.IsNullOrEmpty(x.NewsPublished.Subtitle) ? x.NewsPublished.Subtitle
                        : x.NewsPublished.Content.ExtractTextFromHtml(),
                    Slug = x.NewsPublished.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                    SlugWithOutClientURL = x.NewsPublished.Slug,
                    ImageURL = x.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                        _appSettingsKeys.CDN_MediaGalleryFolder)
                })
                .ToList();

            bool hasMoreItems = categoryNews.Count > pageSize;

            if (hasMoreItems)
            {
                categoryNews.RemoveAt(categoryNews.Count - 1);
            }

            result.Data.Items = categoryNews;
            result.Data.HasMoreItems = hasMoreItems;

            return result;
        }

        public async Task<ServiceResult<ZodiacSignBlockCacheResponse>> GetZodiacSignNewsListForIndex()
        {
            var result = new ServiceResult<ZodiacSignBlockCacheResponse>
            {
                Data = new ZodiacSignBlockCacheResponse
                {
                    ZodiacSignItems = new List<ZodiacSignBlockCacheResponseItem>()
                }
            };

            var zodiacSignResponse = await _newsCachingService.GetMainPageZodiacSignBlock();

            if (zodiacSignResponse != null)
            {
                result.Data = zodiacSignResponse.Data;
                return result;
            }

            return result;
        }

        public Task<ServiceResult> VoteSurvey(int surveyId, int optionId)
        {
            var result = new ServiceResult();

            _ = Task.Run(async () =>
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var newsCachingService = scope.ServiceProvider.GetRequiredService<INewsCachingService>();
                        await newsCachingService.IncreaseSurveyResult(surveyId, optionId);
                    }
                }
                catch
                {
                }
            });

            return Task.FromResult(result);
        }

        public async Task<ServiceResult<SurveyOptionResultCachingResponse>> GetSurveyOptionResult(int surveyID)
        {
            var result = new ServiceResult<SurveyOptionResultCachingResponse>();

            var cachingSurveyResponse = await _newsCachingService.GetSurveyOptionResult(surveyID);
            if (!cachingSurveyResponse.IsSuccess)
            {
                var surveyoptions = await _context.SurveyOptions.Where(x => x.SurveyID == surveyID).ToListAsync();
                var totalAnswers = surveyoptions.Sum(y => y.AnswerCount);

                var surveyoptionResult = surveyoptions.Select(x => new SurveyOptionsResult()
                {
                    OptionID = x.ID,
                    AnswerCount = x.AnswerCount,
                    PercentRatio = totalAnswers == 0 ? 0 : (int)Math.Round(((double)x.AnswerCount / totalAnswers) * 100)
                }).ToList();


                var surveyOptionResultCachingResponse = new SurveyOptionResultCachingResponse()
                {
                    SurveyID = surveyID,
                    SurveyOptions = surveyoptionResult
                };
                result.Data = surveyOptionResultCachingResponse;
            }
            else
            {
                result.Data = cachingSurveyResponse.Data;
            }

            return result;
        }

        public async Task<ServiceResult<string>> GetUrlByID(int id)
        {
            var result = new ServiceResult<string>();

            id = id.ToString().TrimStart('-').ToInt();

            var cacheResponse = await _newsCachingService.GetNewsDetailByID(id);

            if (!cacheResponse.IsSuccess)
            {
                var isPublished = await _context.NewsPublished.AnyAsync(x => x.NewsID == id);
                if (!isPublished)
                {
                    result.SetError("Haber bulunamadı.");
                    return result;
                }

                var news = await (from n in _context.NewsPublished
                                  join cat in _context.Categories on n.CategoryID equals cat.ID
                                  where n.NewsID == id
                                  select new
                                  {
                                      Slug = n.Slug,
                                  }
                    ).FirstOrDefaultAsync();

                string slugResponse = news.Slug.TrimStart('/');
                result.Data = $"/{slugResponse}";

                return result;
            }

            string slug = cacheResponse.Data.NewsSlug.TrimStart('/');

            result.Data = $"/{slug}";
            return result;
        }

        public async Task<ServiceResult<GetNewsListResponse>> GetMobileNewsByCategorySlug(int categoryID)
        {
            var result = new ServiceResult<GetNewsListResponse>
            {
                Data = new GetNewsListResponse
                {
                    SurveyNewsTwo = new List<SurveyNews>(),
                    SurveyNewsSix = new List<SurveyNews>(),
                    SurveyNewsOthers = new List<SurveyNews>(),
                    CategoryTwo = new List<CategoryNews>(),
                    CategorySix = new List<CategoryNews>(),
                    CategoryTen = new List<CategoryNews>(),
                    CategoryOthers = new List<CategoryNews>(),
                    ZodiacSignNews = new List<CategoryNews>(),
                    ZodiacSignNewsForSlider = new List<ZodiacSignNewsForSlider>(),
                    MobileCategoryNews = new List<CategoryNews>(),
                }
            };

            var publishedNews = await (from cat in _context.Categories
                                       where cat.ID == categoryID
                                       select new GetNewsListResponse
                                       {
                                           CategoryBreadcrumb = cat.Breadcrumb,
                                           CategoryName = cat.Name,
                                           CategoryID = cat.ID,
                                           CategorySlug = cat.Url.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                       }).FirstOrDefaultAsync();


            if (categoryID != (int)CategoryEnum.Survey
                && categoryID != (int)CategoryEnum.GovernmentAnnouncement
                && categoryID != (int)CategoryEnum.Astrologie)
            {
                var categoryNews = await (from np in _context.NewsPublished
                                          let photo = (from ph in _context.NewsPublishedPhotos
                                                       where ph.NewsID == np.NewsID &&
                                                             (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                              ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                                       orderby
                                                           ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                               ? 0
                                                               : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                                                 !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                   ? 1
                                                                   : 2
                                                       select ph).FirstOrDefault()
                                          where np.CategoryID == categoryID
                                          orderby np.PublishedDate descending
                                          select new CategoryNews
                                          {
                                              ID = (int)np.NewsID,
                                              Headline = !string.IsNullOrEmpty(np.Headline) ? np.Headline
                                                  : !string.IsNullOrEmpty(np.Title) ? np.Title
                                                  : null,
                                              UpSubtitle = !string.IsNullOrEmpty(np.UpSubtitle) ? np.UpSubtitle
                                                  : !string.IsNullOrEmpty(np.Subtitle) ? np.Subtitle
                                                  : np.Content.ExtractTextFromHtml(),
                                              Slug = np.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                              ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                                  _appSettingsKeys.CDN_MediaGalleryFolder),
                                          }).Take(30).ToListAsync();

                publishedNews.MobileCategoryNews = categoryNews;
            }

            if (categoryID == (int)CategoryEnum.Astrologie)
            {
                var zodiacSignNews = await (from np in _context.NewsPublished
                                            let photo = (from ph in _context.NewsPublishedPhotos
                                                         where ph.NewsID == np.NewsID &&
                                                               (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                                ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                                         orderby
                                                             ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                 ? 0
                                                                 : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                                                   !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                     ? 1
                                                                     : 2
                                                         select ph).FirstOrDefault()
                                            where np.CategoryID == categoryID
                                            orderby np.PublishedDate descending
                                            select new CategoryNews
                                            {
                                                ID = (int)np.NewsID,
                                                UpHeadline = np.UpHeadline,
                                                Headline = !string.IsNullOrEmpty(np.Headline) ? np.Headline
                                                    : !string.IsNullOrEmpty(np.Title) ? np.Title
                                                    : null,
                                                UpSubtitle = !string.IsNullOrEmpty(np.UpSubtitle) ? np.UpSubtitle
                                                    : !string.IsNullOrEmpty(np.Subtitle) ? np.Subtitle
                                                    : np.Content.ExtractTextFromHtml(),
                                                Slug = np.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                                ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                                    _appSettingsKeys.CDN_MediaGalleryFolder),
                                                PublishDate = np.PublishedDate.ToFormattedDate()
                                            }).Take(30).ToListAsync();


                publishedNews.MobileCategoryNews = zodiacSignNews.Skip(10).ToList();
                publishedNews.ZodiacSignNewsForSlider = zodiacSignNews.Take(10).Select(x => new ZodiacSignNewsForSlider
                {
                    Slug = x.Slug,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    ShowUpHeadline = !string.IsNullOrEmpty(x.UpHeadline),
                    ImageURL = x.ImageURL
                }).ToList();
            }

            if (categoryID == (int)CategoryEnum.Survey)
            {
                var surveyNews = await _context.NewsPublished
                    .Where(np => np.NewsTypeID == (int)NewsTypesEnum.Survey)
                    .Select(np => new
                    {
                        NewsPublishedForSurvey = np,
                        NewsVersions = _context.NewsVersions
                            .Where(nv => nv.NewsID == np.NewsID)
                            .OrderByDescending(nv => nv.ID)
                            .ToList()
                    })
                    .ToListAsync();


                var allSurveyNews = surveyNews
                    .Select(x => new
                    {
                        x.NewsPublishedForSurvey,
                        LatestSurvey = x.NewsVersions
                            .Select(nv => _context.Surveys
                                .Where(s => s.NewsVersionID == nv.ID)
                                .OrderByDescending(s => s.ID)
                                .FirstOrDefault())
                            .FirstOrDefault(s => s != null)
                    })
                    .Where(x => x.LatestSurvey != null)
                    .Select(x => new SurveyNews
                    {
                        ID = x.LatestSurvey.ID,
                        Title = x.NewsPublishedForSurvey.Title ?? x.LatestSurvey.Question,
                        Question = x.LatestSurvey.Question ?? x.NewsPublishedForSurvey.Title,
                        Slug = _appSettingsKeys.Common_ClientWebSite + x.NewsPublishedForSurvey.Slug.TrimStart('/'),
                        ImageURL = _context.NewsPublishedPhotos
                            .Where(p => p.NewsID == x.NewsPublishedForSurvey.NewsID &&
                                        p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto)
                            .FirstOrDefault().PhotoUrl != null
                            ? _context.NewsPublishedPhotos
                                .Where(p => p.NewsID == x.NewsPublishedForSurvey.NewsID &&
                                            p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto)
                                .FirstOrDefault().PhotoUrl
                                .PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder)
                            : _context.NewsPublishedPhotos
                                .Where(p => p.NewsID == x.NewsPublishedForSurvey.NewsID &&
                                            p.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                .FirstOrDefault().PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                    _appSettingsKeys.CDN_MediaGalleryFolder),
                        RemainingTime = x.LatestSurvey.EndDate.DateToShortCountdown(x.LatestSurvey.StartDate),
                        JoinedUserCount = (int)_context.SurveyOptions
                            .Where(so => so.SurveyID == x.LatestSurvey.ID)
                            .Sum(so => so.AnswerCount),
                    })
                    .OrderByDescending(sn => sn.JoinedUserCount)
                    .ToList();

                if (allSurveyNews.Any())
                {
                    publishedNews.SurveyNewsOthers = allSurveyNews.ToList();
                }
            }

            if (categoryID == (int)CategoryEnum.GovernmentAnnouncement)
            {
                var governmentAnnouncementNews = await (from np in _context.NewsPublished
                                                        let photo = (from ph in _context.NewsPublishedPhotos
                                                                     where ph.NewsID == np.NewsID &&
                                                                           (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                                            ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                                                     orderby
                                                                         ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                             ? 0
                                                                             : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                                                               !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                                 ? 1
                                                                                 : 2
                                                                     select ph).FirstOrDefault()
                                                        where np.CategoryID == categoryID
                                                        orderby np.PublishedDate descending
                                                        select new CategoryNews
                                                        {
                                                            ID = (int)np.NewsID,
                                                            Headline = !string.IsNullOrEmpty(np.Headline) ? np.Headline
                                                                : !string.IsNullOrEmpty(np.Title) ? np.Title
                                                                : null,
                                                            Slug = np.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                                            ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                                                _appSettingsKeys.CDN_MediaGalleryFolder)
                                                        }).Take(32).ToListAsync();

                publishedNews.GovernmentAnnouncementNews = governmentAnnouncementNews;
            }

            result.Data = publishedNews;
            return result;
        }

        public async Task<ServiceResult<List<GovernmentAnnouncementItem>>> GetDailyGovernmentAnnouncement()
        {
            var result = new ServiceResult<List<GovernmentAnnouncementItem>>
            {
                Data = new List<GovernmentAnnouncementItem>()
            };

            var governmentAnnouncements = await _newsCachingService.GetMainPageGovernmentAnnouncement();

            if (governmentAnnouncements != null)
            {
                result.Data = governmentAnnouncements.Data;
                return result;
            }

            return result;
        }

        public async Task<ServiceResult<List<GovernmentAnnouncementItem>>> GetGovernmentAnnouncementListByDate(
            DateTime dateTime)
        {
            var result = new ServiceResult<List<GovernmentAnnouncementItem>>
            {
                Data = new List<GovernmentAnnouncementItem>()
            };

            var governmentAnnouncementList = await (
                from np in _context.NewsPublished
                let photo = (from ph in _context.NewsPublishedPhotos
                             where ph.NewsID == np.NewsID &&
                                   (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                    ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                             orderby
                                 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                     ? 0
                                     : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                         ? 1
                                         : 2
                             select ph).FirstOrDefault()
                where np.NewsTypeID == (int)NewsTypesEnum.GovernmentAnnouncement &&
                      np.PublishedDate.Date == dateTime.Date
                select new GovernmentAnnouncementItem
                {
                    ID = (int)np.NewsID,
                    Headline = np.Headline ?? np.Title,
                    UpSubtitle = np.UpSubtitle ?? np.Subtitle ?? np.Content.ExtractTextFromHtml(),
                    Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                    CoverPhoto = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                        _appSettingsKeys.CDN_MediaGalleryFolder),
                    BoxCount = np.BoxCount ?? 0,
                    PressNo = np.PressNo,
                    OrderGroup = np.BoxCount >= 150 ? (int)GovernmentOrderGroupEnum.UnderAnaSlider : 0
                }).ToListAsync();

            result.Data = governmentAnnouncementList;
            return result;
        }

        public Task<ServiceResult> AddViewCount(int newsID)
        {
            var result = new ServiceResult();

            _ = Task.Run(async () =>
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var newsCachingService = scope.ServiceProvider.GetRequiredService<INewsCachingService>();
                        await newsCachingService.IncreaseNewsViewCount(newsID);
                    }
                }
                catch
                {
                }
            });

            // Hemen success dön 
            return Task.FromResult(result);
        }

        //TODO: KATEGORI SORGULARINDA BU KULLANILACAKTIR.
        private async Task<List<CategoryNews>> GetTopCategoryNewsAsync(List<CategoryEnum> categoryEnums, int count = 4)
        {
            var result = new List<CategoryNews>();

            var categoryIds = categoryEnums.Select(c => (int)c).ToList();

            result = await (
                from np in _context.NewsPublished
                join n in _context.News on np.NewsID equals n.ID
                where categoryIds.Contains(np.CategoryID ?? -1)
                let photo = (from ph in _context.NewsPublishedPhotos
                             where ph.NewsID == np.NewsID &&
                                   (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                    ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                             orderby
                                 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                     ? 0
                                     : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                         ? 1
                                         : 2
                             select ph).FirstOrDefault()
                orderby n.ViewCount descending
                select new CategoryNews
                {
                    ID = (int)np.NewsID,
                    Headline = np.Headline ?? np.Title,
                    UpSubtitle = np.UpSubtitle ?? np.Subtitle ?? np.Content.ExtractTextFromHtml(),
                    Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                    ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                        _appSettingsKeys.CDN_MediaGalleryFolder),
                }).Take(count).ToListAsync();

            return result;
        }

        private async Task<ServiceResult<GetNewsDetailResponse>> GetNewsDetailService(int ID)
        {
            var result = new ServiceResult<GetNewsDetailResponse>();

            var newsPublished = await _context.NewsPublished.Where(np => np.NewsID == ID).AnyAsync();

            if (!newsPublished)
            {
                result.SetError("News not found");
                return result;
            }

            var news = await (
                from np in _context.NewsPublished
                join t in _context.NewsTypes on np.NewsTypeID equals t.ID
                join c in _context.Categories on np.CategoryID equals c.ID into categories
                from category in categories.DefaultIfEmpty()
                where np.NewsID == ID
                select new GetNewsDetailResponse
                {
                    ID = ID,
                    NewsSlug = np.Slug,
                    NewsTypeID = np.NewsTypeID,
                    Title = np.Title,
                    SubTitle = np.Subtitle ?? np.UpSubtitle,
                    Content = np.Content,
                    Headline = np.Headline ?? np.Title,
                    UpHeadline = np.UpHeadline,
                    UpdateDate = np.ManuelUpdatedDate ?? DateTime.Now,
                    CreatedDate = np.PublishedDate,
                    PublishDate = np.PublishedDate,
                    SourceIDs = np.SourceIDs,
                    AuthorID = np.AuthorID,
                    CategoryID = np.CategoryID ?? 0,
                    PressNo = np.PressNo,
                    BoxCount = np.BoxCount,
                    CategorySlug = _appSettingsKeys.Common_ClientWebSite + (category != null ? category.Url : ""),
                    CategoryName = category.Name ?? string.Empty,
                    CategoryBreadcrumb = category.Breadcrumb ?? category.Name,
                    Video = np.Video,
                    VideoPlayerID = np.VideoPlayerID,
                    SecondaryCategoryMap = np.SecondaryCategoryMap,
                    SeoDescription = !string.IsNullOrEmpty(np.SeoDescription)
                        ? np.SeoDescription
                        : np.Content.ExtractTextFromHtml(),
                    SeoTitle = !string.IsNullOrEmpty(np.SeoTitle) ? np.SeoTitle : np.Title,
                }).FirstOrDefaultAsync();


            if (news == null)
            {
                result.Message = "Haber bulunamadı.";
                return result;
            }

            if (news.PublishDate > news.UpdateDate)
            {
                news.UpdateDate = news.PublishDate;
            }

            if (string.IsNullOrEmpty(news.SubTitle))
            {
                news.SubTitle = news.Content.ExtractTextFromHtml(160);
            }

            var offsetUpdatedDate = new DateTimeOffset(news.UpdateDate, TimeSpan.FromHours(3));
            var offsetUPublishedDate = new DateTimeOffset(news.PublishDate, TimeSpan.FromHours(3));
            var formattedUpdateDate =
                offsetUpdatedDate.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
            var formattedPublishedDate =
                offsetUPublishedDate.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);

            news.UpdateDateForMetaTag = formattedUpdateDate;
            news.PublishDateForMetaTag = formattedPublishedDate;
            news.Tags = news.TagList != null ? string.Join(", ", news.TagList.Select(x => x.Text)) : string.Empty;

            #region Yazar İşlemleri

            if (news.NewsTypeID == (int)NewsTypesEnum.ColumnArticles ||
                news.NewsTypeID == (int)NewsTypesEnum.CaricaturistNews)
            {
                var authors = await _context.Authors.Select(x => new AuthorCachingResponse
                {
                    ID = x.ID,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname,
                    Url = x.Url,
                    Photo = x.ClichePhoto,
                    AuthorSocialFacebook = x.FacebookUrl,
                    AuthorSocialInstagram = x.InstagramUrl,
                    AuthorSocialLinkedIn = x.LinkedinUrl,
                    AuthorSocialMail = x.Email,
                    AuthorSocialX = x.XUrl
                }).ToListAsync();

                var author = authors.FirstOrDefault(a => a.ID == news.AuthorID);

                news.AuthorName = author != null
                    ? $"{author.Firstname} {author.Lastname}".Trim()
                    : string.Empty;
                news.AuthorSlug = author != null
                    ? _appSettingsKeys.Common_ClientWebSite + author.Url.TrimStart('/')
                    : string.Empty;

                if (news.NewsTypeID == (int)NewsTypesEnum.ColumnArticles && author != null)
                {
                    var authorsLastArticles = await (
                        from np in _context.NewsPublished
                        where np.AuthorID == author.ID && np.NewsTypeID == (int)NewsTypesEnum.ColumnArticles
                        orderby np.PublishedDate descending
                        select new AuthorsLastColumnArticles
                        {
                            NewsID = (int)np.NewsID,
                            Headline = np.Headline ?? np.Title,
                            UpSubtitle = np.UpSubtitle ?? np.Subtitle,
                            Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                            PublishDate = np.PublishedDate.ToFormattedDate()
                        }).Take(6).ToListAsync();

                    news.ColumnArticleDetails = new ColumnArticleDetails
                    {
                        AuthorFullName = news.AuthorName,
                        AuthorSlug = _appSettingsKeys.Common_ClientWebSite + author.Url.TrimStart('/'),
                        AuthorPhoto = author.Photo.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                            _appSettingsKeys.CDN_AuthorPhotos),
                        AuthorSocialFacebook = author.AuthorSocialFacebook,
                        AuthorSocialInstagram = author.AuthorSocialInstagram,
                        AuthorSocialX = author.AuthorSocialX,
                        AuthorSocialLinkedIn = author.AuthorSocialLinkedIn,
                        AuthorSocialMail = author.AuthorSocialMail,
                        AuthorsLastColumnArticles = authorsLastArticles
                    };
                }
            }

            #endregion

            #region Foto Galeri

            if (news.NewsTypeID == (int)NewsTypesEnum.PhotoGallery)
            {
                news.PhotoGalleryItems = await _context.NewsPublishedPhotoGalleryItems
                    .Where(x => x.NewsID == ID)
                    .OrderBy(x => x.Order)
                    .Select(x => new PhotoGalleryItem
                    {
                        ImageURL = x.PhotoUrl,
                        Content = x.Content
                    }).ToListAsync();
            }

            #endregion

            #region İkincil Kategoriler

            var secondaryCategoryIds =
                await _context.NewsSecondaryCategories.Where(x => x.NewsID == news.ID).ToListAsync();
            if (secondaryCategoryIds.Any())
            {
                news.SecondaryCategoryIDList = secondaryCategoryIds.Select(x => x.SecondaryCategoryID).ToList();
                news.SecondaryCategoryNameList = await _context.Categories
                    .Where(x => news.SecondaryCategoryIDList.Contains(x.ID))
                    .Select(x => x.Name).ToListAsync();
            }

            #endregion

            #region Kaynaklar

            var sourceIds = await _context.NewsSources.Where(x => x.NewsID == news.ID).Select(x => x.SourceID)
                .ToListAsync();
            if (sourceIds.Any())
            {
                news.SourceList = await _context.Sources
                    .Where(x => sourceIds.Contains(x.ID))
                    .Select(x => new SourceList
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Slug = _appSettingsKeys.Common_ClientWebSite + _appSettingsKeys.SlugPrefix_Sources + "/" +
                               x.Url.TrimStart('/')
                    }).ToListAsync();

                news.Sources = string.Join(", ", news.SourceList.Select(x => x.Name));
            }

            #endregion

            #region Kapak Fotoğrafı

            var coverPhoto = await _context.NewsPublishedPhotos
                .Where(p => p.NewsID == ID &&
                            (p.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto ||
                             p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto))
                .OrderBy(p => p.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto ? 0 : 1)
                .FirstOrDefaultAsync();

            if (news.NewsTypeID == (int)NewsTypesEnum.ColumnArticles
                && coverPhoto.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
            {
                news.CoverPhoto = coverPhoto != null
                    ? coverPhoto.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_AuthorPhotos)
                    : string.Empty;
            }
            else
            {
                news.CoverPhoto = coverPhoto != null
                    ? coverPhoto.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                        _appSettingsKeys.CDN_MediaGalleryFolder)
                    : string.Empty;
            }

            #endregion

            #region Etiketler

            news.TagList = await (from nt in _context.NewsTags
                                  join t in _context.Tags on nt.TagID equals t.ID
                                  where nt.NewsID == ID
                                  select new TagList
                                  {
                                      ID = t.ID,
                                      Text = t.Text,
                                      Slug = _appSettingsKeys.Common_ClientWebSite + _appSettingsKeys.SlugPrefix_Tags + "/" +
                                             t.Url.TrimStart('/')
                                  }).ToListAsync();

            #endregion

            #region Kategori Haberleri

            news.CategoryNewsList = await (
                from np in _context.NewsPublished
                join n in _context.News on np.NewsID equals n.ID
                let photo = (from ph in _context.NewsPublishedPhotos
                             where ph.NewsID == np.NewsID &&
                                   (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                    ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                             orderby
                                 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                     ? 0
                                     : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                         ? 1
                                         : 2
                             select ph).FirstOrDefault()
                orderby n.ViewCount descending
                select new CategoryNews
                {
                    ID = (int)np.NewsID,
                    Headline = np.Headline ?? np.Title,
                    UpSubtitle = np.UpSubtitle ?? np.Subtitle ?? np.Content.ExtractTextFromHtml(),
                    Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                    ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                        _appSettingsKeys.CDN_MediaGalleryFolder),
                }).Take(4).ToListAsync();

            #endregion

            #region İlişkili Haberler

            news.RelatedNewsList = await (from n in _context.RelatedNews
                                          join np in _context.NewsPublished on n.RelatedNewsID equals np.NewsID
                                          let photo = (from ph in _context.NewsPublishedPhotos
                                                       where ph.NewsID == np.NewsID &&
                                                             (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                              ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                                       orderby
                                                           ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                               ? 0
                                                               : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                   ? 1
                                                                   : 2
                                                       select ph).FirstOrDefault()
                                          where n.NewsID == ID && !string.IsNullOrEmpty(np.Slug)
                                          orderby n.Order
                                          select new Models.Responses.NewsResponses.RelatedNews
                                          {
                                              ID = (int)n.NewsID,
                                              Title = np.Title,
                                              Slug = _appSettingsKeys.Common_ClientWebSite + np.Slug.TrimStart('/'),
                                              Subtitle = np.Subtitle,
                                              ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                                  _appSettingsKeys.CDN_MediaGalleryFolder),
                                          }).ToListAsync();

            #endregion

            #region Anketler

            if (news.NewsTypeID == (int)NewsTypesEnum.Survey)
            {
                var newsVersionIds = await _context.NewsVersions
                    .Where(x => x.NewsID == news.ID)
                    .OrderByDescending(x => x.ID)
                    .Select(x => x.ID)
                    .ToListAsync();

                Survey survey = null;

                foreach (var versionId in newsVersionIds)
                {
                    survey = await _context.Surveys
                        .Where(x => x.NewsVersionID == versionId)
                        .FirstOrDefaultAsync();


                    if (survey != null)
                    {
                        break;
                    }
                }

                var surveyOptionList = await _context.SurveyOptions.Where(x =>
                        x.SurveyID == survey.ID && x.NewsVersionID == (int)survey.NewsVersionID)
                    .ToListAsync();

                var totalAnswerCount = surveyOptionList.Sum(x => x.AnswerCount);

                var surveyOptions = surveyOptionList
                    .OrderBy(x => x.Order)
                    .Select(x => new SurveySection
                    {
                        Option = x.Option,
                        Percent = totalAnswerCount > 0
                            ? (int)Math.Round((double)x.AnswerCount / totalAnswerCount * 100)
                            : 0,
                        OptionID = x.ID,
                        SurveyID = x.SurveyID,
                    })
                    .ToList();


                var surveyNews = await _context.NewsPublished
                    .Where(np => np.NewsTypeID == (int)NewsTypesEnum.Survey)
                    .Select(np => new
                    {
                        NewsPublishedForSurvey = np,
                        NewsVersions = _context.NewsVersions
                                .Where(nv => nv.NewsID == np.NewsID)
                                .OrderByDescending(nv => nv.ID)
                            .ToList()
                    })
                    .ToListAsync();


                var finalResult = surveyNews
                    .Select(x => new
                    {
                        x.NewsPublishedForSurvey,
                        LatestSurvey = x.NewsVersions
                                .Select(nv => _context.Surveys
                                    .Where(s => s.NewsVersionID == nv.ID)
                                    .OrderByDescending(s => s.ID)
                                    .FirstOrDefault())
                                .FirstOrDefault(s => s != null)
                    })
                        .Where(x => x.LatestSurvey != null)
                        .Select(x => new SurveyNews
                        {
                            ID = x.LatestSurvey.ID,
                            Title = x.NewsPublishedForSurvey.Title ?? x.LatestSurvey.Question,
                            Question = x.LatestSurvey.Question ?? x.NewsPublishedForSurvey.Title,
                            Slug = _appSettingsKeys.Common_ClientWebSite + x.NewsPublishedForSurvey.Slug.TrimStart('/'),
                            ImageURL = _context.NewsPublishedPhotos
                                .Where(p => p.NewsID == x.NewsPublishedForSurvey.NewsID &&
                                            p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto)
                                .FirstOrDefault().PhotoUrl != null
                                ? _context.NewsPublishedPhotos
                                    .Where(p => p.NewsID == x.NewsPublishedForSurvey.NewsID &&
                                                p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto)
                                    .FirstOrDefault().PhotoUrl
                                    .PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder)
                                : _context.NewsPublishedPhotos
                                    .Where(p => p.NewsID == x.NewsPublishedForSurvey.NewsID &&
                                                p.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                    .FirstOrDefault().PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                        _appSettingsKeys.CDN_MediaGalleryFolder),
                            RemainingTime = x.LatestSurvey.EndDate.DateToShortCountdown(x.LatestSurvey.StartDate),
                            JoinedUserCount = (int)_context.SurveyOptions
                                .Where(so => so.SurveyID == x.LatestSurvey.ID)
                                .Sum(so => so.AnswerCount),
                        })
                    .OrderByDescending(sn => sn.JoinedUserCount)
                    .Take(7)
                    .ToList();

                news.SurveyElements = new SurveyElements
                {
                    JoinedUserCount = (int)surveyOptionList.Select(x => x.AnswerCount).ToList().Sum(),
                    RemainingTime = survey.EndDate.DateToLongCountdown(survey.StartDate),
                    SurveyQuestion = survey.Question,
                    SurveySections = surveyOptions,
                    CategoryNews = news.CategoryNewsList.Take(3).ToList(),
                    SurveyNewsThird = finalResult.Take(3).ToList(),
                    SurveyNewsQuad = finalResult.Skip(3).Take(4).ToList()
                };
            }

            #endregion

            #region Scrool News

            var randomNewsIds =
                await GetRandomRelatedNewsIds(news.ID, news.NewsTypeID, news.CategoryID, news.SecondaryCategoryMap);
            news.ScrollNewsIDs = string.Join(",", randomNewsIds);
            news.NewsDetailNextNewsIDs = randomNewsIds;

            #endregion

            result.Data = news;
            return result;
        }

        public async Task<ServiceResult<string>> CheckURLToRedirect(string slug, string contextURL)
        {
            var result = new ServiceResult<string>();

            if (slug == null)
            {
                result.SetError("Haber bulunamadı.");
                return result;
            }

            if (!slug.TrimStart('/').Equals(contextURL.TrimStart('/')))
            {
                result.SetError("URL'ler uyuşmuyor.");
                result.Data = slug;
            }
            result.Data = slug;
            return result;
        }

        private async Task<ServiceResult<GetNewsListResponse>> CreateGetNewsByCategorySlug(int categoryID)
        {
            var result = new ServiceResult<GetNewsListResponse>
            {
                Data = new GetNewsListResponse
                {
                    SurveyNewsTwo = new List<SurveyNews>(),
                    SurveyNewsSix = new List<SurveyNews>(),
                    SurveyNewsOthers = new List<SurveyNews>(),
                    CategoryTwo = new List<CategoryNews>(),
                    CategorySix = new List<CategoryNews>(),
                    CategoryTen = new List<CategoryNews>(),
                    CategoryOthers = new List<CategoryNews>(),
                    ZodiacSignNews = new List<CategoryNews>(),
                    ZodiacSignNewsForSlider = new List<ZodiacSignNewsForSlider>(),
                }
            };

            var publishedNews = await (from cat in _context.Categories
                                       where cat.ID == categoryID
                                       select new GetNewsListResponse
                                       {
                                           CategoryBreadcrumb = cat.Breadcrumb,
                                           CategoryName = cat.Name,
                                           CategoryID = cat.ID,
                                           CategorySlug = cat.Url.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                           SeoDescription = !string.IsNullOrEmpty(cat.SeoDescription) ? cat.SeoDescription : cat.Name,
                                           SeoTitle = !string.IsNullOrEmpty(cat.SeoTitle) ? cat.SeoTitle : cat.Name,
                                       }).FirstOrDefaultAsync();

            if (categoryID != (int)CategoryEnum.Survey
                && categoryID != (int)CategoryEnum.GovernmentAnnouncement
                && categoryID != (int)CategoryEnum.Astrologie)
            {
                var categoryNews = await (from np in _context.NewsPublished
                                          let photo = (from ph in _context.NewsPublishedPhotos
                                                       where ph.NewsID == np.NewsID &&
                                                             (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                              ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                                       orderby
                                                           ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                               ? 0
                                                               : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                                                 !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                   ? 1
                                                                   : 2
                                                       select ph).FirstOrDefault()
                                          where np.CategoryID == categoryID
                                          orderby np.PublishedDate descending
                                          select new CategoryNews
                                          {
                                              ID = (int)np.NewsID,
                                              Headline = !string.IsNullOrEmpty(np.Headline) ? np.Headline
                                                  : !string.IsNullOrEmpty(np.Title) ? np.Title
                                                  : null,
                                              UpSubtitle = !string.IsNullOrEmpty(np.UpSubtitle) ? np.UpSubtitle
                                                  : !string.IsNullOrEmpty(np.Subtitle) ? np.Subtitle
                                                  : np.Content.ExtractTextFromHtml(),
                                              Slug = np.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                              ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                                  _appSettingsKeys.CDN_MediaGalleryFolder),
                                          }).Take(30).ToListAsync();

                publishedNews.CategoryTwo = categoryNews.Take(2).ToList();
                publishedNews.CategorySix = categoryNews.Skip(2).Take(6).ToList();
                publishedNews.CategoryOthers = categoryNews.Skip(8).Take(22).ToList();

            }

            if (categoryID == (int)CategoryEnum.Astrologie)
            {
                var zodiacSignNews = await (from np in _context.NewsPublished
                                            let photo = (from ph in _context.NewsPublishedPhotos
                                                         where ph.NewsID == np.NewsID &&
                                                               (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                                ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                                         orderby
                                                             ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                 ? 0
                                                                 : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                                                   !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                     ? 1
                                                                     : 2
                                                         select ph).FirstOrDefault()
                                            where np.CategoryID == categoryID
                                            orderby np.PublishedDate descending
                                            select new CategoryNews
                                            {
                                                ID = (int)np.NewsID,
                                                UpHeadline = np.UpHeadline,
                                                Headline = !string.IsNullOrEmpty(np.Headline) ? np.Headline
                                                    : !string.IsNullOrEmpty(np.Title) ? np.Title
                                                    : null,
                                                UpSubtitle = !string.IsNullOrEmpty(np.UpSubtitle) ? np.UpSubtitle
                                                    : !string.IsNullOrEmpty(np.Subtitle) ? np.Subtitle
                                                    : np.Content.ExtractTextFromHtml(),
                                                Slug = np.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                                ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                                    _appSettingsKeys.CDN_MediaGalleryFolder),
                                                PublishDate = np.PublishedDate.ToFormattedDate()
                                            }).Take(30).ToListAsync();


                publishedNews.ZodiacSignNews = zodiacSignNews.Skip(10).ToList();
                publishedNews.ZodiacSignNewsForSlider = zodiacSignNews.Take(10).Select(x => new ZodiacSignNewsForSlider
                {
                    Slug = x.Slug,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    ShowUpHeadline = !string.IsNullOrEmpty(x.UpHeadline),
                    ImageURL = x.ImageURL
                }).ToList();
            }

            if (categoryID == (int)CategoryEnum.Survey)
            {
                var surveyNews = await _context.NewsPublished
                    .Where(np => np.NewsTypeID == (int)NewsTypesEnum.Survey)
                    .Select(np => new
                    {
                        NewsPublishedForSurvey = np,
                        NewsVersions = _context.NewsVersions
                            .Where(nv => nv.NewsID == np.NewsID)
                            .OrderByDescending(nv => nv.ID)
                            .ToList()
                    })
                    .ToListAsync();


                var allSurveyNews = surveyNews
                    .Select(x => new
                    {
                        x.NewsPublishedForSurvey,
                        LatestSurvey = x.NewsVersions
                            .Select(nv => _context.Surveys
                                .Where(s => s.NewsVersionID == nv.ID)
                                .OrderByDescending(s => s.ID)
                                .FirstOrDefault())
                            .FirstOrDefault(s => s != null)
                    })
                    .Where(x => x.LatestSurvey != null)
                    .Select(x => new SurveyNews
                    {
                        ID = x.LatestSurvey.ID,
                        Title = x.NewsPublishedForSurvey.Title ?? x.LatestSurvey.Question,
                        Question = x.LatestSurvey.Question ?? x.NewsPublishedForSurvey.Title,
                        Slug = _appSettingsKeys.Common_ClientWebSite + x.NewsPublishedForSurvey.Slug.TrimStart('/'),
                        ImageURL = _context.NewsPublishedPhotos
                            .Where(p => p.NewsID == x.NewsPublishedForSurvey.NewsID &&
                                        p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto)
                            .FirstOrDefault().PhotoUrl != null
                            ? _context.NewsPublishedPhotos
                                .Where(p => p.NewsID == x.NewsPublishedForSurvey.NewsID &&
                                            p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto)
                                .FirstOrDefault().PhotoUrl
                                .PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder)
                            : _context.NewsPublishedPhotos
                                .Where(p => p.NewsID == x.NewsPublishedForSurvey.NewsID &&
                                            p.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                .FirstOrDefault().PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                    _appSettingsKeys.CDN_MediaGalleryFolder),
                        RemainingTime = x.LatestSurvey.EndDate.DateToShortCountdown(x.LatestSurvey.StartDate),
                        JoinedUserCount = (int)_context.SurveyOptions
                            .Where(so => so.SurveyID == x.LatestSurvey.ID)
                            .Sum(so => so.AnswerCount),
                    })
                    .OrderByDescending(sn => sn.JoinedUserCount)
                    .ToList();

                if (allSurveyNews.Any())
                {
                    publishedNews.SurveyNewsTwo = allSurveyNews.Take(2).ToList();
                    publishedNews.SurveyNewsSix = allSurveyNews.Skip(2).Take(8).ToList();
                    publishedNews.SurveyNewsOthers = allSurveyNews.Skip(8).ToList();
                }

            }

            if (categoryID == (int)CategoryEnum.GovernmentAnnouncement)
            {
                var governmentAnnouncementNews = await (from np in _context.NewsPublished
                                                        let photo = (from ph in _context.NewsPublishedPhotos
                                                                     where ph.NewsID == np.NewsID &&
                                                                           (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                                            ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                                                     orderby
                                                                         ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                             ? 0
                                                                             : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                                                               !string.IsNullOrEmpty(ph.PhotoUrl)
                                                                                 ? 1
                                                                                 : 2
                                                                     select ph).FirstOrDefault()
                                                        where np.CategoryID == categoryID && np.PublishedDate.Date == DateTime.Now.Date
                                                        orderby np.PublishedDate descending
                                                        select new CategoryNews
                                                        {
                                                            ID = (int)np.NewsID,
                                                            Headline = !string.IsNullOrEmpty(np.Headline) ? np.Headline
                                                                : !string.IsNullOrEmpty(np.Title) ? np.Title
                                                                : null,
                                                            Slug = np.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                                            ImageURL = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                                                _appSettingsKeys.CDN_MediaGalleryFolder),
                                                            PressNo = np.PressNo,
                                                        }).Take(32).ToListAsync();

                publishedNews.GovernmentAnnouncementNews = governmentAnnouncementNews;
            }

            result.Data = publishedNews;
            return result;
        }

        private async Task<ServiceResult<GetNewsListResponse>> CreateGetNewsBySourceSlug(string sourceSlug)
        {
            var result = new ServiceResult<GetNewsListResponse>
            {
                Data = new GetNewsListResponse
                {
                    SurveyNewsTwo = new List<SurveyNews>(),
                    SurveyNewsSix = new List<SurveyNews>(),
                    SurveyNewsOthers = new List<SurveyNews>(),
                    CategoryTwo = new List<CategoryNews>(),
                    CategorySix = new List<CategoryNews>(),
                    CategoryTen = new List<CategoryNews>(),
                    CategoryOthers = new List<CategoryNews>(),
                    ZodiacSignNews = new List<CategoryNews>(),
                    ZodiacSignNewsForSlider = new List<ZodiacSignNewsForSlider>(),
                }
            };


            var sourceWithResponse = await (from src in _context.Sources
                                            where src.Url == sourceSlug
                                            select new GetNewsListResponse
                                            {
                                                SourceBreadcrumb = src.Breadcrumb,
                                                SourceName = src.Name,
                                                SourceID = src.ID,
                                                SourceSlug = src.Url.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                                SourceSlugWithoutClientURL = src.Url
                                            }).FirstOrDefaultAsync();

            if (sourceWithResponse == null)
            {
                result.SetError("Kaynak bulunamadı.");
                return result;
            }

            //Source id boş ise?
            int sourceId = sourceWithResponse.SourceID;

            var categoryNews = await (from np in _context.NewsPublished
                                      let photo = (from ph in _context.NewsPublishedPhotos
                                                   where ph.NewsID == np.NewsID &&
                                                         (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                          ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                                   orderby
                                                       ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                                           ? 0
                                                           : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                                             !string.IsNullOrEmpty(ph.PhotoUrl)
                                                               ? 1
                                                               : 2
                                                   select ph).FirstOrDefault()
                                      where
                                          np.SourceIDs == sourceId.ToString() ||
                                          np.SourceIDs.StartsWith(sourceId + ",") ||
                                          np.SourceIDs.Contains("," + sourceId + ",") ||
                                          np.SourceIDs.EndsWith("," + sourceId)
                                      orderby
                                          np.PublishedDate descending
                                      select new
                                      {
                                          np,
                                          photo = photo.PhotoUrl
                                      })
                .Take(29)
                .ToListAsync();

            var finalResult = categoryNews.Select(x => new CategoryNews
            {
                ID = (int)x.np.NewsID,
                Headline = !string.IsNullOrEmpty(x.np.Headline) ? x.np.Headline
                    : !string.IsNullOrEmpty(x.np.Title) ? x.np.Title
                    : null,
                UpSubtitle = !string.IsNullOrEmpty(x.np.UpSubtitle) ? x.np.UpSubtitle
                    : !string.IsNullOrEmpty(x.np.Subtitle) ? x.np.Subtitle
                    : x.np.Content.ExtractTextFromHtml(),
                Slug = x.np.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                SlugWithOutClientURL = x.np.Slug,
                ImageURL = x.photo?.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                    _appSettingsKeys.CDN_MediaGalleryFolder),
            }).ToList();

            sourceWithResponse.CategoryNine = finalResult.Take(9).ToList();
            sourceWithResponse.CategoryOthers = finalResult.Skip(9).Take(20).ToList();

            result.Data = sourceWithResponse;
            return result;
        }
        private async Task<ServiceResult<GetNewsListResponse>> CreateGetNewsByTagSlug(string tagSlug)
        {
            var result = new ServiceResult<GetNewsListResponse>
            {
                Data = new GetNewsListResponse
                {
                    SurveyNewsTwo = new List<SurveyNews>(),
                    SurveyNewsSix = new List<SurveyNews>(),
                    SurveyNewsOthers = new List<SurveyNews>(),
                    CategoryTwo = new List<CategoryNews>(),
                    CategorySix = new List<CategoryNews>(),
                    CategoryTen = new List<CategoryNews>(),
                    CategoryOthers = new List<CategoryNews>(),
                    ZodiacSignNews = new List<CategoryNews>(),
                    ZodiacSignNewsForSlider = new List<ZodiacSignNewsForSlider>(),
                }
            };

            var tagWithResponse = await (from t in _context.Tags
                                         where t.Url.Contains(tagSlug)
                                         select new GetNewsListResponse
                                         {
                                             TagBreadcrumb = t.Breadcrumb,
                                             TagName = t.Text,
                                             TagID = t.ID,
                                             TagSlugWithoutClientURL = t.Url,
                                             TagSlug = t.Url.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                                             SeoDescription = t.SeoDescription,
                                             SeoTitle = t.SeoTitle,
                                         }).FirstOrDefaultAsync();

            if (tagWithResponse == null)
            {
                result.SetError("Etiket bulunamadı.");
                return result;
            }


            var newsWithTag = await _context.NewsTags
                .Where(nt => nt.TagID == tagWithResponse.TagID)
                .Select(nt => nt.NewsID)
                .Distinct()
                .ToListAsync();

            var categoryNews = await _context.NewsPublished
                .Where(np => newsWithTag.Contains(np.NewsID))
                .OrderByDescending(np => np.PublishedDate)
                .Take(29)
                .Select(np => new
                {
                    NewsPublished = np,
                    PhotoUrl = _context.NewsPublishedPhotos
                        .Where(ph => ph.NewsID == np.NewsID &&
                                     (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                      ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto))
                        .OrderBy(ph =>
                            ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl)
                                ? 0
                                : ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto &&
                                  !string.IsNullOrEmpty(ph.PhotoUrl)
                                    ? 1
                                    : 2)
                        .Select(ph => ph.PhotoUrl)
                        .FirstOrDefault()
                })
                .ToListAsync();

            var mappedCategoryNews = categoryNews
                .Select(x => new CategoryNews
                {
                    ID = (int)x.NewsPublished.NewsID,
                    Headline = !string.IsNullOrEmpty(x.NewsPublished.Headline) ? x.NewsPublished.Headline
                        : !string.IsNullOrEmpty(x.NewsPublished.Title) ? x.NewsPublished.Title
                        : null,
                    UpSubtitle = !string.IsNullOrEmpty(x.NewsPublished.UpSubtitle) ? x.NewsPublished.UpSubtitle
                        : !string.IsNullOrEmpty(x.NewsPublished.Subtitle) ? x.NewsPublished.Subtitle
                        : x.NewsPublished.Content,
                    Slug = x.NewsPublished.Slug.PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite),
                    SlugWithOutClientURL = x.NewsPublished.Slug,
                    ImageURL = x.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                        _appSettingsKeys.CDN_MediaGalleryFolder)
                })
                .ToList();

            tagWithResponse.CategoryNine = mappedCategoryNews.Take(9).ToList();
            tagWithResponse.CategoryOthers = mappedCategoryNews.Skip(9).Take(20).ToList();

            result.Data = tagWithResponse;
            return result;
        }

        public static GetNewsListResponse MapToResponse(GetNewsListCachingResponse source)
        {
            return new GetNewsListResponse
            {
                SeoDescription = source.SeoDescription,
                SeoTitle = source.SeoTitle,
                CategoryID = source.CategoryID,
                CategorySlug = source.CategorySlug,
                CategoryBreadcrumb = source.CategoryBreadcrumb,
                CategoryName = source.CategoryName,

                SourceID = source.SourceID,
                SourceSlug = source.SourceSlug,
                SourceSlugWithoutClientURL = source.SourceSlugWithoutClientURL,
                SourceBreadcrumb = source.SourceBreadcrumb,
                SourceName = source.SourceName,

                TagID = source.TagID,
                TagSlug = source.TagSlug,
                TagSlugWithoutClientURL = source.TagSlugWithoutClientURL,
                TagBreadcrumb = source.TagBreadcrumb,
                TagName = source.TagName,

                CategoryTwo = source.CategoryTwo.Select(x => new CategoryNews
                {
                    ID = x.ID,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    Slug = x.Slug,
                    SlugWithOutClientURL = x.SlugWithOutClientURL,
                    UpSubtitle = x.UpSubtitle,
                    ImageURL = x.ImageURL,
                    PublishDate = x.PublishDate,
                    PressNo = x.PressNo
                }).ToList(),

                CategorySix = source.CategorySix.Select(x => new CategoryNews
                {
                    ID = x.ID,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    Slug = x.Slug,
                    SlugWithOutClientURL = x.SlugWithOutClientURL,
                    UpSubtitle = x.UpSubtitle,
                    ImageURL = x.ImageURL,
                    PublishDate = x.PublishDate,
                    PressNo = x.PressNo
                }).ToList(),

                CategoryNine = source.CategoryNine.Select(x => new CategoryNews
                {
                    ID = x.ID,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    Slug = x.Slug,
                    SlugWithOutClientURL = x.SlugWithOutClientURL,
                    UpSubtitle = x.UpSubtitle,
                    ImageURL = x.ImageURL,
                    PublishDate = x.PublishDate,
                    PressNo = x.PressNo
                }).ToList(),

                CategoryTen = source.CategoryTen.Select(x => new CategoryNews
                {
                    ID = x.ID,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    Slug = x.Slug,
                    SlugWithOutClientURL = x.SlugWithOutClientURL,
                    UpSubtitle = x.UpSubtitle,
                    ImageURL = x.ImageURL,
                    PublishDate = x.PublishDate,
                    PressNo = x.PressNo
                }).ToList(),

                ZodiacSignNews = source.ZodiacSignNews.Select(x => new CategoryNews
                {
                    ID = x.ID,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    Slug = x.Slug,
                    SlugWithOutClientURL = x.SlugWithOutClientURL,
                    UpSubtitle = x.UpSubtitle,
                    ImageURL = x.ImageURL,
                    PublishDate = x.PublishDate,
                    PressNo = x.PressNo
                }).ToList(),

                CategoryOthers = source.CategoryOthers.Select(x => new CategoryNews
                {
                    ID = x.ID,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    Slug = x.Slug,
                    SlugWithOutClientURL = x.SlugWithOutClientURL,
                    UpSubtitle = x.UpSubtitle,
                    ImageURL = x.ImageURL,
                    PublishDate = x.PublishDate,
                    PressNo = x.PressNo
                }).ToList(),

                MobileCategoryNews = source.MobileCategoryNews.Select(x => new CategoryNews
                {
                    ID = x.ID,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    Slug = x.Slug,
                    SlugWithOutClientURL = x.SlugWithOutClientURL,
                    UpSubtitle = x.UpSubtitle,
                    ImageURL = x.ImageURL,
                    PublishDate = x.PublishDate,
                    PressNo = x.PressNo
                }).ToList(),

                GovernmentAnnouncementNews = source.GovernmentAnnouncementNews.Select(x => new CategoryNews
                {
                    ID = x.ID,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    Slug = x.Slug,
                    SlugWithOutClientURL = x.SlugWithOutClientURL,
                    UpSubtitle = x.UpSubtitle,
                    ImageURL = x.ImageURL,
                    PublishDate = x.PublishDate,
                    PressNo = x.PressNo
                }).ToList(),

                SurveyNewsTwo = source.SurveyNewsTwo.Select(x => new SurveyNews
                {
                    ID = x.ID,
                    Title = x.Title,
                    Question = x.Question,
                    Slug = x.Slug,
                    ImageURL = x.ImageURL,
                    RemainingTime = x.RemainingTime,
                    JoinedUserCount = x.JoinedUserCount
                }).ToList(),

                SurveyNewsSix = source.SurveyNewsSix.Select(x => new SurveyNews
                {
                    ID = x.ID,
                    Title = x.Title,
                    Question = x.Question,
                    Slug = x.Slug,
                    ImageURL = x.ImageURL,
                    RemainingTime = x.RemainingTime,
                    JoinedUserCount = x.JoinedUserCount
                }).ToList(),

                SurveyNewsOthers = source.SurveyNewsOthers.Select(x => new SurveyNews
                {
                    ID = x.ID,
                    Title = x.Title,
                    Question = x.Question,
                    Slug = x.Slug,
                    ImageURL = x.ImageURL,
                    RemainingTime = x.RemainingTime,
                    JoinedUserCount = x.JoinedUserCount
                }).ToList(),

                ZodiacSignNewsForSlider = source.ZodiacSignNewsForSlider.Select(x => new ZodiacSignNewsForSlider
                {
                    Slug = x.Slug,
                    Headline = x.Headline,
                    UpHeadline = x.UpHeadline,
                    ShowUpHeadline = x.ShowUpHeadline,
                    ImageURL = x.ImageURL
                }).ToList()
            };
        }

        public async Task<ServiceResult<GetCachingNewsDetailResponse>> CreateNewsDetail(int id)
        {
            var result = new ServiceResult<GetCachingNewsDetailResponse>();

            var detail = await _newsCachingService.CreateNewsDetail(id);
            if (detail == null)
            {
                result.SetError("Haber bulunamadı.");
                return result;
            }

            result.Data = detail.Data;

            return result;
        }

        public async Task<ServiceResult> TestMethod()
        {
            var result = new ServiceResult();

            var increasedElasticDatas = await _newsCachingService.GetIncreasedNewsViewCount();
            if (increasedElasticDatas.Data.Any())
            {
                var increasedDatas = increasedElasticDatas.Data.Select(x => new
                {
                    Count = x.Count,
                    NewsID = x.NewsID,
                }).ToList();

                foreach (var data in increasedDatas)
                {
                    var news = _context.News.FirstOrDefault(x => x.ID == data.NewsID);
                    if (news == null)
                    {
                        continue;
                    }

                    if (news.ViewCount != null)
                    {
                        news.ViewCount = +data.Count;
                    }
                    else
                    {
                        news.ViewCount = 0;
                    }
                    _context.News.Update(news);
                }
                await _context.SaveChangesAsync();
            }
            return result;
        }
    }
}