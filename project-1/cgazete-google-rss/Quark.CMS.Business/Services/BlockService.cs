using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.NewsCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Cloudflare.Adapter.Interfaces;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.BlockRequests;
using Quark.CMS.Business.Models.Responses.BlockResponses;
using Quark.CMS.Business.Models.Responses.BlokResponses;
using Quark.CMS.Business.Models.Responses.CommonResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.BlockLog;
using Quark.Data.Models.News;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class BlockService : BaseService, IBlockService
{
    private readonly IBlockCachingService _blockCachingService;
    private readonly IUserCachingService _userCachingService;
    private readonly IPageCachingService _pageCachingService;
    private readonly INewsCachingService _newsCachingService;
    private readonly ICloudflareAdapter _cloudflareAdapter;

    public BlockService(IHttpContextAccessor httpContext, QuarkDataContext context,
        IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache,
        IUserCachingService userCachingService,
        IBlockCachingService blockCachingService,
        IPageCachingService pageCachingService,
        INewsCachingService newsCachingService,
        ICloudflareAdapter cloudflareAdapter) : base(httpContext, context,
        localizer, appSettingsKeys, cache)
    {
        _blockCachingService = blockCachingService;
        _pageCachingService = pageCachingService;
        _userCachingService = userCachingService;
        _newsCachingService = newsCachingService;
        _cloudflareAdapter = cloudflareAdapter;
    }

    public async Task<ServiceResult<ListFiltersRequest>> GetAllPagedFilters()
    {
        var result = new ServiceResult<ListFiltersRequest>();
        result.Data = new ListFiltersRequest();

        var pagesList = await _pageCachingService.PagesAsSelectListItems();
        result.Data.Pages = pagesList.Data;
        result.Data.Pages.AddALLOption();

        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseBlokItem>>> GetAllPaged(BaseDataTableRequest tableRequest,
        ListFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<ListResponseBlokItem>>();

        result.RecordsTotal = _context.Blocks.Count();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        var query = (from b in _context.Blocks
                     orderby b.Order ?? int.MaxValue
                     join bt in _context.BlockTypes on b.BlockTypeID equals bt.ID
                     join p in _context.Pages on b.PageID equals p.ID
                     where request.SelectedPageID.IsALLSelected() || request.SelectedPageID == p.ID
                     orderby b.Order ?? int.MaxValue
                     select new
                     {
                         ID = b.ID,
                         Name = b.Name,
                         IsEnabled = b.IsEnabled,
                         IsEnabledText = b.IsEnabled ? enabledText : disabledText,
                         AutoSlideDelay = b.AutoSlideDelay,
                         BlockType = bt.Name,
                         PageName = p.Name
                     }).AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Name.ToUpper().Contains(tableRequest.SearchValue.ToUpper()) ||
                x.PageName.ToUpper().Contains(tableRequest.SearchValue.ToUpper()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseBlokItem
        {
            ID = x.ID,
            Name = x.Name,
            IsEnabled = x.IsEnabled,
            IsEnabledText = x.IsEnabledText,
            BlockType = x.BlockType,
            PageName = x.PageName
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetAllSelectList(int? pageId = null)
    {
        var result = new ServiceResult<List<SelectListItem>>();

        var blocks = await (from b in _context.Blocks
                            let bUsers = (from bu in _context.BlockUsers
                                          where bu.BlockID == b.ID
                                          select bu.UserID).ToList()
                            where b.IsEnabled && (pageId == null || b.PageID == pageId)
                            where bUsers.Contains(CurrentUserID)
                            orderby b.Order
                            select new SelectListItem
                            {
                                Value = b.ID.ToString(),
                                Text = b.Name
                            }).AsNoTracking().ToListAsync();

        result.Data = blocks.AddSELECTOption();

        return result;
    }

    public async Task<ServiceResult<MinMaxValuesResponse>> GetMinMaxValues(int blockId)
    {
        var result = new ServiceResult<MinMaxValuesResponse>();

        var block = await _context.Blocks.FirstOrDefaultAsync(x => x.ID == blockId);

        if (block == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        result.Data = new MinMaxValuesResponse
        {
            Min = block.MinNewsCount,
            Max = block.MaxNewsCount
        };

        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> Create()
    {
        var result = new ServiceResult<CreateUpdateRequest>();

        var users = await _userCachingService.Users();
        var blockTypes = await _blockCachingService.BlockTypes();
        var pages = await _pageCachingService.PagesAsSelectListItems();
        var categories = await _context.Categories.Where(x => x.IsEnabled && x.ParentID == null).Select(x => new SelectListItem
        {
            Value = x.ID.ToString(),
            Text = x.Name,
        }).ToListAsync();

        var createBlock = new CreateUpdateRequest
        {
            Categories = categories,
            Users = users.Data,
            BlockTypes = blockTypes.Data,
            Pages = pages.Data
        };

        result.Data = createBlock;
        result.Data.Pages.AddSELECTOption();
        result.Data.Categories.AddSELECTOption();
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> Create(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest> { Data = request };

        var isBlockExist = await _context.Blocks.Where(x => x.Name == request.Name.Trim()).AnyAsync();

        if (isBlockExist)
        {
            var users = await _userCachingService.Users();
            var blockTypes = await _blockCachingService.BlockTypes();
            var pages = await _pageCachingService.PagesAsSelectListItems();
            var categories = await _context.Categories.Where(x => x.IsEnabled && x.ParentID == null).Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name,
            }).ToListAsync();

            result.Data.Categories = categories;
            result.Data.Users = users.Data;
            result.Data.BlockTypes = blockTypes.Data;
            result.Data.Pages = pages.Data;
            result.Data.Pages.AddSELECTOption();
            result.Data.Categories.AddSELECTOption();
            result.SetError(_localizer["RecordWithNameAlreadyExist"]);
            return result;
        }

        var isKeyExist = await _context.Blocks.Where(x => x.Key == request.Key.Trim()).AnyAsync();

        if (isKeyExist)
        {
            var users = await _userCachingService.Users();
            var blockTypes = await _blockCachingService.BlockTypes();
            var pages = await _pageCachingService.PagesAsSelectListItems();
            var categories = await _context.Categories.Where(x => x.IsEnabled && x.ParentID == null).Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name,
            }).ToListAsync();

            result.Data.Categories = categories;
            result.Data.Users = users.Data;
            result.Data.BlockTypes = blockTypes.Data;
            result.Data.Pages = pages.Data;
            result.Data.Pages.AddSELECTOption();
            result.Data.Categories.AddSELECTOption();
            result.SetError(_localizer["RecordWithKeyAlreadyExists"]);
            return result;
        }

        if (request.MinNewsCount > request.MaxNewsCount)
        {
            var users = await _userCachingService.Users();
            var blockTypes = await _blockCachingService.BlockTypes();
            var pages = await _pageCachingService.PagesAsSelectListItems();
            var categories = await _context.Categories.Where(x => x.IsEnabled && x.ParentID == null).Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name,
            }).ToListAsync();

            result.Data.Categories = categories;
            result.Data.Users = users.Data;
            result.Data.BlockTypes = blockTypes.Data;
            result.Data.Pages = pages.Data;
            result.Data.Pages.AddSELECTOption();
            result.Data.Categories.AddSELECTOption();

            result.SetError(_localizer["MinValueCanNotBeGreaterThanMaxValue"]);
            return result;
        }

        var block = new Block
        {
            CategoryID = request.SelectedCategoryID,
            Name = request.Name.Trim(),
            Key = request.Key.Trim(),
            AutoSlideDelay = request.AutoSliderDelay,
            MaxNewsCount = request.MaxNewsCount,
            MinNewsCount = request.MinNewsCount,
            IsEnabled = request.IsEnabled,
            BlockTypeID = int.Parse(request.BlockTypeID),
            Title = request.Title,
            PageID = request.SelectedPageID,
            Order = request.Order,
        };

        await _context.Blocks.AddAsync(block);
        await _context.SaveChangesAsync();

        foreach (var userID in request.SelectedUserIDs)
        {
            var blockUser = new BlockUser
            {
                UserID = userID,
                BlockID = block.ID
            };
            await _context.BlockUsers.AddAsync(blockUser);
        }

        await _context.SaveChangesAsync();

        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> Update(int id)
    {
        var result = new ServiceResult<CreateUpdateRequest>();

        var block = await (from b in _context.Blocks
                           where b.ID == id
                           select new CreateUpdateRequest
                           {
                               ID = b.ID,
                               Name = b.Name,
                               Key = b.Key,
                               IsEnabled = b.IsEnabled,
                               MinNewsCount = b.MinNewsCount,
                               MaxNewsCount = b.MaxNewsCount,
                               Title = b.Title,
                               AutoSliderDelay = b.AutoSlideDelay,
                               BlockTypeID = b.BlockTypeID.ToString(),
                               SelectedPageID = b.PageID,
                               SelectedCategoryID = b.CategoryID,
                               Order = b.Order,
                           }).FirstOrDefaultAsync();

        if (block is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var users = await _userCachingService.Users();
        var blockTypes = await _blockCachingService.BlockTypes();
        var pages = await _pageCachingService.PagesAsSelectListItems();
        var categories = await _context.Categories.Where(x => x.IsEnabled && x.ParentID == null).Select(x => new SelectListItem
        {
            Value = x.ID.ToString(),
            Text = x.Name,
        }).ToListAsync();

        var maxOrder = await _context.Blocks.MaxAsync(x => x.Order);

        block.Order = block.Order ?? (maxOrder + 1);
        block.Users = users.Data;
        block.BlockTypes = blockTypes.Data;
        block.Pages = pages.Data;
        block.Categories = categories;
        block.Pages.AddSELECTOption();
        block.Categories.AddSELECTOption();
        block.SelectedUserIDs =
            await _context.BlockUsers.Where(x => x.BlockID == id).Select(x => x.UserID).ToListAsync();

        result.Data = block;
        return result;
    }

    public async Task<ServiceResult<CreateUpdateRequest>> Update(CreateUpdateRequest request)
    {
        var result = new ServiceResult<CreateUpdateRequest> { Data = request };

        var isBlockExist = await _context.Blocks.Where(x => x.Name == request.Name.Trim() && x.ID != request.ID)
            .AnyAsync();

        if (isBlockExist)
        {
            var users = await _userCachingService.Users();
            var blockTypes = await _blockCachingService.BlockTypes();
            var pages = await _pageCachingService.PagesAsSelectListItems();
            var categories = await _context.Categories.Where(x => x.IsEnabled && x.ParentID == null).Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name,
            }).ToListAsync();

            result.Data.Categories = categories;
            result.Data.Users = users.Data;
            result.Data.BlockTypes = blockTypes.Data;
            result.Data.Pages = pages.Data;
            result.Data.Pages.AddSELECTOption();
            result.Data.Categories.AddSELECTOption();
            result.SetError(_localizer["RecordWithNameAlreadyExist"]);
            return result;
        }

        var isKeyExist = await _context.Blocks.Where(x => x.Key == request.Key.Trim() && x.ID != request.ID).AnyAsync();

        if (isKeyExist)
        {
            var users = await _userCachingService.Users();
            var blockTypes = await _blockCachingService.BlockTypes();
            var pages = await _pageCachingService.PagesAsSelectListItems();
            var categories = await _context.Categories.Where(x => x.IsEnabled && x.ParentID == null).Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name,
            }).ToListAsync();

            result.Data.Categories = categories;
            result.Data.Users = users.Data;
            result.Data.BlockTypes = blockTypes.Data;
            result.Data.Pages = pages.Data;
            result.Data.Pages.AddSELECTOption();
            result.Data.Categories.AddSELECTOption();
            result.SetError(_localizer["RecordWithKeyAlreadyExists"]);
            return result;
        }

        if (request.MinNewsCount > request.MaxNewsCount)
        {
            var users = await _userCachingService.Users();
            var blockTypes = await _blockCachingService.BlockTypes();
            var pages = await _pageCachingService.PagesAsSelectListItems();
            var categories = await _context.Categories.Where(x => x.IsEnabled && x.ParentID == null).Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name,
            }).ToListAsync();

            result.Data.Categories = categories;
            result.Data.Users = users.Data;
            result.Data.BlockTypes = blockTypes.Data;
            result.Data.Pages = pages.Data;
            result.Data.Pages.AddSELECTOption();
            result.Data.Categories.AddSELECTOption();
            result.SetError(_localizer["MinValueCanNotBeGreaterThanMaxValue"]);
            return result;
        }

        var block = await _context.Blocks.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (block is null)
        {
            var users = await _userCachingService.Users();
            var blockTypes = await _blockCachingService.BlockTypes();
            var pages = await _pageCachingService.PagesAsSelectListItems();
            var categories = await _context.Categories.Where(x => x.IsEnabled && x.ParentID == null).Select(x => new SelectListItem
            {
                Value = x.ID.ToString(),
                Text = x.Name,
            }).ToListAsync();

            result.Data.Categories = categories;
            result.Data.Users = users.Data;
            result.Data.BlockTypes = blockTypes.Data;
            result.Data.Pages = pages.Data;
            result.Data.Pages.AddSELECTOption();
            result.Data.Categories.AddSELECTOption();
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        block.CategoryID = request.SelectedCategoryID;
        block.Name = request.Name;
        block.MaxNewsCount = request.MaxNewsCount;
        block.MinNewsCount = request.MinNewsCount;
        block.Key = request.Key;
        block.Title = request.Title;
        block.IsEnabled = request.IsEnabled;
        block.AutoSlideDelay = request.AutoSliderDelay;
        block.BlockTypeID = int.Parse(request.BlockTypeID);
        block.PageID = request.SelectedPageID;
        block.Order = request.Order;

        var blockUsers = await _context.BlockUsers.Where(x => x.BlockID == request.ID).ToListAsync();
        _context.RemoveRange(blockUsers);
        foreach (var userID in request.SelectedUserIDs)
        {
            var blockUser = new BlockUser
            {
                UserID = userID,
                BlockID = block.ID
            };
            await _context.BlockUsers.AddAsync(blockUser);
        }

        _context.Blocks.Update(block);
        await _context.SaveChangesAsync();
        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }

    public async Task<ServiceResult> Delete(int id)
    {
        var result = new ServiceResult();

        var block = await _context.Blocks.FirstOrDefaultAsync(x => x.ID == id);
        if (block is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var blockUsers = await _context.BlockUsers.Where(x => x.BlockID == id).ToListAsync();

        _context.BlockUsers.RemoveRange(blockUsers);

        block.IsEnabled = false;
        _context.Blocks.Update(block);
        await _context.SaveChangesAsync();

        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }

    public async Task<PagingResponse<ListBlockNewsResponseItem>> GetNewsPaged(GetBlockNewsFiltersRequest request)
    {
        var result = new PagingResponse<ListBlockNewsResponseItem>();
        //var cacheNewsList = await _newsCachingService.GetBlockNewsList();

        //var query = (from cn in cacheNewsList.Data
        //    where request.SelectedNewsItems == null ||
        //          !request.SelectedNewsItems.Select(x => x.NewsID).Contains(cn.ID)
        //    let inUseBlockList = (from bi in _context.BlockItems
        //        where bi.NewsID == cn.ID
        //        join b in _context.Blocks on bi.BlockID equals b.ID
        //        orderby b.Order ?? int.MaxValue
        //        select b.Name).Distinct().ToList()
        //        select new
        //    {
        //        ID = cn.ID,
        //        Title = cn.Title,
        //        PhotoUrl = cn.CoverPhoto,
        //        Category = cn.CategoryName,
        //        PublishedDate = cn.PublishDate,
        //        Tags = cn.Tags,
        //        InUse = inUseBlockList.Any(),
        //        InUseBlocksList = inUseBlockList
        //    }).ToList();

        var query = (from cn in _context.NewsPublished
                     join c in _context.Categories on cn.CategoryID equals c.ID into category
                     from c in category.DefaultIfEmpty()
                     let photo = (from ph in _context.NewsPhotos
                                  where ph.NewsID == cn.NewsID &&
                                        (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                         ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                  orderby
                                      ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
                                      ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 : 2
                                  select ph).FirstOrDefault()
                     where (request.SelectedNewsItems == null ||
                            !request.SelectedNewsItems.Select(x => x.NewsID).Contains(cn.NewsID))
                       && (cn.PublishedDate > DateTime.Today.AddDays(-15) || cn.ManuelUpdatedDate > DateTime.Today.AddDays(-15))
                     let inUseBlockList = (from bi in _context.BlockItems
                                           where bi.NewsID == cn.NewsID
                                           join b in _context.Blocks on bi.BlockID equals b.ID
                                           orderby b.Order ?? int.MaxValue
                                           select b.Name).ToList()
                     orderby cn.PublishedDate descending
                     select new
                     {
                         ID = cn.NewsID,
                         Title = cn.Title,
                         PhotoUrl = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder),
                         Category = c.Name,
                         PublishedDate = cn.PublishedDate,
                         Tags = cn.Tags,
                         InUse = inUseBlockList.Any(),
                         InUseBlocksList = inUseBlockList
                     }).AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(x =>
                x.Title.ToUpper().Contains(request.SearchValue.ToUpper()) ||
                x.Category.ToUpper().Contains(request.SearchValue.ToUpper()) ||
                x.ID.ToString().Contains(request.SearchValue));

            query = query.Take(200);
        }
        else
        {
            query = query.Take(1000);
        }

        result.PageCount = (int)Math.Ceiling((query.Count()) / (decimal)request.PageSize);
        result.CurrentPage = request.Page;
        result.HasNext = result.CurrentPage < result.PageCount;
        result.HasPrevious = result.CurrentPage > 1;
        result.SearchValue = request.SearchValue == null ? string.Empty : request.SearchValue;

        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        result.Data = items.Select(x => new ListBlockNewsResponseItem
        {
            ID = x.ID,
            Title = x.Title,
            Category = x.Category,
            PhotoUrl = x.PhotoUrl,
            PublishDate = x.PublishedDate.ToFormattedDateTime(),
            InUse = x.InUse,
            InUseBlockList = x.InUseBlocksList
        }).ToList();

        return result;
    }

    public async Task<ServiceResult> SaveBlockNews(CreateBlockNewsRequest request)
    {
        var result = new ServiceResult();

        try
        {
            var block = await _context.Blocks.Where(x => x.ID == request.BlockID).FirstOrDefaultAsync();
            if (block is null)
            {
                result.SetError(_localizer["RecordWithNameAlreadyExist"]);
                return result;
            }

            var existingBlockItems = await _context.BlockItems
                .Where(x => x.BlockID == block.ID)
                .ToListAsync();

            var newItems = request.SelectedNewsItems;

            var addedItems = newItems
                .Where(n => !existingBlockItems.Any(e => e.NewsID == n.NewsID))
                .ToList();

            var deletedItems = existingBlockItems
                .Where(e => !newItems.Any(n => n.NewsID == e.NewsID))
                .ToList();

            var updatedItems = newItems
                .Where(n => existingBlockItems.Any(e => e.NewsID == n.NewsID && e.Order != n.Order))
                .ToList();

            if (!addedItems.Any() && !deletedItems.Any() && !updatedItems.Any())
            {
                return result;
            }

            var logHistory = await _context.BlockLogs
                .Where(log => log.BlockID == block.ID)
                .GroupBy(log => log.NewsID)
                .Select(g => g.OrderByDescending(log => log.CreatedDate).FirstOrDefault())
                .ToDictionaryAsync(log => log.NewsID);

            foreach (var item in addedItems)
            {
                await _context.BlockLogs.AddAsync(new BlockLog
                {
                    BlockID = block.ID,
                    NewsID = item.NewsID,
                    ChangeType = "Added",
                    UserID = CurrentUserID,
                    OldOrder = null,
                    NewOrder = item.Order,
                    CreatedDate = DateTime.Now,
                    PreviousChangeDate = null,
                    Duration = null
                });
            }

            foreach (var item in deletedItems)
            {
                var previousLog = logHistory.ContainsKey(item.NewsID) ? logHistory[item.NewsID] : null;
                await _context.BlockLogs.AddAsync(new BlockLog
                {
                    BlockID = block.ID,
                    NewsID = item.NewsID,
                    ChangeType = "Deleted",
                    UserID = CurrentUserID,
                    OldOrder = item.Order,
                    NewOrder = null,
                    CreatedDate = DateTime.Now,
                    PreviousChangeDate = previousLog?.CreatedDate,
                    Duration = previousLog != null ? (long)(DateTime.Now - previousLog.CreatedDate).TotalSeconds : null
                });
            }

            foreach (var item in updatedItems)
            {
                var oldItem = existingBlockItems.First(e => e.NewsID == item.NewsID);
                var previousLog = logHistory.ContainsKey(item.NewsID) ? logHistory[item.NewsID] : null;

                await _context.BlockLogs.AddAsync(new BlockLog
                {
                    BlockID = block.ID,
                    NewsID = item.NewsID,
                    ChangeType = "Updated",
                    UserID = CurrentUserID,
                    OldOrder = oldItem.Order,
                    NewOrder = item.Order,
                    CreatedDate = DateTime.Now,
                    PreviousChangeDate = previousLog?.CreatedDate,
                    Duration = previousLog != null ? (long)(DateTime.Now - previousLog.CreatedDate).TotalSeconds : null
                });
            }

            await _context.BlockItems.Where(x => x.BlockID == block.ID).ExecuteDeleteAsync();

            foreach (var item in request.SelectedNewsItems)
            {
                BlockItem blockItem = new()
                {
                    BlockID = block.ID,
                    NewsID = item.NewsID,
                    Order = item.Order,
                };

                await _context.BlockItems.AddAsync(blockItem);
            }

            await _context.SaveChangesAsync();
            await _blockCachingService.RefreshBlockNewsWithKey(block.Key);
            _cloudflareAdapter.Purge(_appSettingsKeys.Common_ClientWebSite);
        }
        catch (Exception e)
        {
            result.SetError(_localizer["Error"]);
        }

        return result;
    }

    public async Task<ServiceResult<List<ListBlockNewsResponseItem>>> GetNewsByBlockId(int blockId)
    {
        var result = new ServiceResult<List<ListBlockNewsResponseItem>>()
        {
            Data = new List<ListBlockNewsResponseItem>()
        };

        result.Data = await (from bi in _context.BlockItems
                             join np in _context.NewsPublished on bi.NewsID equals np.NewsID
                             join c in _context.Categories on np.CategoryID equals c.ID into category
                             from c in category.DefaultIfEmpty()
                             let photo = (from ph in _context.NewsPhotos
                                          where ph.NewsID == np.NewsID &&
                                                (ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto ||
                                                 ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto)
                                          orderby
                                              ph.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 0 :
                                              ph.PhotoTypeID == (int)NewsPhotoTypesEnum.CoverPhoto && !string.IsNullOrEmpty(ph.PhotoUrl) ? 1 : 2
                                          select ph).FirstOrDefault()
                             where bi.BlockID == blockId
                             orderby bi.Order ascending
                             select new ListBlockNewsResponseItem
                             {
                                 ID = bi.NewsID,
                                 Title = np.Title,
                                 PhotoUrl = photo.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl,
                                     _appSettingsKeys.CDN_MediaGalleryFolder) ?? _appSettingsKeys.URL_EmptyPhoto.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder),
                                 Category = c.Name,
                                 PublishDate = np.PublishedDate.ToFormattedDateTime(),
                             }).AsNoTracking().ToListAsync();

        return result;
    }
}