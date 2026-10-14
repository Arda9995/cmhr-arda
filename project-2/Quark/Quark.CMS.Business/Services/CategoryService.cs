using System.Security.Cryptography;
using AvvaMobile.Core.Business;
using Microsoft.EntityFrameworkCore;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.FtpUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CategoryRequests;
using Quark.CMS.Business.Models.Responses.CategoryResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.LogModels;
using Quark.Data.Models.SystemLog;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;
using Quark.Data.Models.Common;
using Quark.Business.Caching.Services;

namespace Quark.CMS.Business.Services;

public class CategoryService(
	IUrlMappingService urlMappingService,
	IHttpContextAccessor httpContext,
	QuarkDataContext context,
	IStringLocalizer<CommonResource> localizer,
	AppSettingsKeys appSettingsKeys,
	ICacheManager cache,
	INewsCachingService newsCachingService,
	StatisticsDataContext statisticsContext,
	FileUpload fileUpload,
	ISystemLogCachingService _systemLogCachingService)
	: BaseService(httpContext, context, localizer, appSettingsKeys, cache), ICategoryService
{
	public ServiceResult<CreateUpdateRequest> Create(int? currentId)
	{
		var result = new ServiceResult<CreateUpdateRequest> { Data = new CreateUpdateRequest() };

		result.Data.Url = _appSettingsKeys.SlugPrefix_Categories;
		result.Data.BaseURL = _appSettingsKeys.Common_ClientWebSite;
		result.Data.ParentID = currentId;

		return result;
	}

	public async Task<ServiceResult<CreateUpdateRequest>> Create(CreateUpdateRequest request)
	{
		var result = new ServiceResult<CreateUpdateRequest> { Data = request };

		var isCategoryExist = await _context.Categories.Where(x => x.Name == request.Name.Trim()).AnyAsync();

		if (isCategoryExist)
		{
			result.SetError(_localizer["RecordAlreadyExist"]);
			return result;
		}

		if (request.Photo != null)
		{
			var uploadResult = fileUpload.Upload(request.Photo, _appSettingsKeys.CDN_CategoryPhotos);
			if (uploadResult.IsSuccess)
			{
				request.PhotoUrl = uploadResult.FileName;
			}
		}

		if (request.BackgroundPhoto != null)
		{
			var uploadResult = fileUpload.Upload(request.BackgroundPhoto, _appSettingsKeys.CDN_CategoryPhotos);
			if (uploadResult.IsSuccess)
			{
				request.BackgroundPhotoUrl = uploadResult.FileName;
			}
		}

		var category = new Category
		{
			Name = request.Name.Trim(),
			IsEnabled = true,
			Url = request.Url,
			SeoTitle = request.SeoTitle,
			SeoDescription = request.SeoDescription,
			Breadcrumb = request.Breadcrumb,
			ParentID = request.ParentID,
			PhotoUrl = request.PhotoUrl,
			BackgroundPhotoUrl = request.BackgroundPhotoUrl
		};

		await _context.Categories.AddAsync(category);
		await _context.SaveChangesAsync();

		if (category.ParentID is null)
		{
			category.Map = "|" + category.ID + "|";
		}
		else
		{
			var parent = await _context.Categories.Where(x => x.ID == category.ParentID).FirstOrDefaultAsync();
			category.Map = parent.Map + category.ID + "|";
		}

		_context.Categories.Update(category);

		var log = new SystemLog
		{
			UserID = CurrentUserID,
			IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
			LogTypeID = (int)LogTypesEnum.Category,
			IsSuccess = true,
			Message = category.Name + " oluşturuldu.",
			ObjectID = category.ID
		};
		await _context.SystemLogs.AddAsync(log);
		await _context.SaveChangesAsync();
		await _systemLogCachingService.RefreshAll();

		result.SetSuccess(_localizer["RecordAdded"]);
		return result;
	}

	public async Task<ServiceResult<CreateUpdateRequest>> Update(int currentId)
	{
		var result = new ServiceResult<CreateUpdateRequest>();

		var category = await (from ct in _context.Categories
							  where ct.ID == currentId
							  select new CreateUpdateRequest
							  {
								  ID = ct.ID,
								  Name = ct.Name,
								  IsEnabled = ct.IsEnabled,
								  ParentID = ct.ParentID,
								  Breadcrumb = ct.Breadcrumb,
								  Url = ct.Url,
								  SeoTitle = ct.SeoTitle,
								  SeoDescription = ct.SeoDescription,
								  BaseURL = _appSettingsKeys.Common_ClientWebSite,
								  PhotoUrl = ct.PhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_CategoryPhotos),
								  BackgroundPhotoUrl = ct.BackgroundPhotoUrl.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_CategoryPhotos)
							  }).FirstOrDefaultAsync();

		if (category is null)
		{
			result.SetError(_localizer["RecordNotFound"]);
			return result;
		}

		result.Data = category;
		return result;
	}

	public async Task<ServiceResult<CreateUpdateRequest>> Update(CreateUpdateRequest request)
	{
		var result = new ServiceResult<CreateUpdateRequest> { Data = request };

		var category = await _context.Categories.FirstOrDefaultAsync(x => x.ID == request.ID);
		if (category is null)
		{
			result.SetError(_localizer["RecordNotFound"]);
			return result;
		}

		var isCategoryExist = await _context.Categories.Where(x => x.Name == request.Name.Trim() && x.ID != request.ID).AnyAsync();

		if (isCategoryExist)
		{
			result.SetError(_localizer["RecordAlreadyExist"]);
			return result;
		}

		if (request.Photo != null)
		{
			var uploadResult = fileUpload.Upload(request.Photo, _appSettingsKeys.CDN_CategoryPhotos);
			if (uploadResult.IsSuccess)
			{
				category.PhotoUrl = uploadResult.FileName;
			}
		}

		if (request.BackgroundPhoto != null)
		{
			var uploadResult = fileUpload.Upload(request.BackgroundPhoto, _appSettingsKeys.CDN_CategoryPhotos);
			if (uploadResult.IsSuccess)
			{
				category.BackgroundPhotoUrl = uploadResult.FileName;
			}
		}

		if (!string.IsNullOrWhiteSpace(category.Url) &&
	   !string.Equals(category.Url, request.Url, StringComparison.OrdinalIgnoreCase))
		{

			var urlMapping = await _context.UrlMappings
				.Where(x => x.NewSlug == category.Url && x.EntityTypeID == (int)EntityTypeEnum.Categories).ToListAsync();

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
				NewSlug = request.Url,
				OldSlug = category.Url,
				EntityTypeID = (int)EntityTypeEnum.Categories,
				CreatedDate = DateTime.Now
			});

			await _context.SaveChangesAsync();
			await urlMappingService.NotifyUrlMappingChanged();
		}

		category.Name = request.Name;
		category.IsEnabled = request.IsEnabled;
		category.Breadcrumb = request.Breadcrumb;
		category.Url = request.Url;
		category.SeoTitle = request.SeoTitle;
		category.SeoDescription = request.SeoDescription;
		category.ParentID = request.ParentID;

		_context.Categories.Update(category);

		var log = new SystemLog
		{
			UserID = CurrentUserID,
			IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
			LogTypeID = (int)LogTypesEnum.Category,
			IsSuccess = true,
			Message = category.Name + " güncellendi.",
			ObjectID = category.ID
		};
		await _context.SystemLogs.AddAsync(log);
		await _context.SaveChangesAsync();
		await _systemLogCachingService.RefreshAll();
		result.SetSuccess(_localizer["RecordUpdated"]);
		return result;
	}

	public async Task<ServiceResult> DeleteAsync(int id)
	{
		var result = new ServiceResult();

		var category = await _context.Categories.FirstOrDefaultAsync(x => x.ID == id);
		if (category is null)
		{
			result.SetError(_localizer["RecordNotFound"]);
			return result;
		}

		_context.Categories.Remove(category);

		var userCategories = await _context.UserCategories.Where(x => x.CategoryID == id).ToListAsync();
		if (userCategories.Count != 0)
		{
			_context.UserCategories.RemoveRange(userCategories);
		}

		var authorCategories = await _context.AuthorCategories.Where(x => x.CategoryID == id).ToListAsync();
		if (authorCategories.Count != 0)
		{
			_context.AuthorCategories.RemoveRange(authorCategories);
		}

		var urlMapping = await _context.UrlMappings.Where(x => x.NewSlug == category.Url && x.EntityTypeID == (int)EntityTypeEnum.Categories).ToListAsync();
		if(urlMapping.Count > 0)
		{
			_context.UrlMappings.RemoveRange(urlMapping);
		}

		var log = new SystemLog
		{
			UserID = CurrentUserID,
			IpAddress = _httpContext.HttpContext.Connection.RemoteIpAddress.ToString(),
			LogTypeID = (int)LogTypesEnum.Category,
			IsSuccess = true,
			Message = category.Name + " silindi.",
			ObjectID = category.ID
		};
		await _context.SystemLogs.AddAsync(log);
		await _context.SaveChangesAsync();
		await _systemLogCachingService.RefreshAll();
		await newsCachingService.RefreshAll();
		await urlMappingService.NotifyUrlMappingChanged();

		result.SetSuccess(_localizer["RecordDeleted"]);
		return result;
	}

	public async Task<ServiceResult<CategoryDetailsResponse>> Details(int id)
	{
		var result = new ServiceResult<CategoryDetailsResponse>();

		var category = await _context.Categories.FirstOrDefaultAsync(x => x.ID == id);

		if (category is null)
		{
			result.SetError(_localizer["RecordNotFound"]);
			return result;
		}

		var request = new CategoryDetailsResponse
		{
			ID = id,
			CategoryName = category.Name,
			PhotoUrl = category.PhotoUrl,
			BackgroundPhotoUrl = category.BackgroundPhotoUrl,
		};

		result.Data = request;
		return result;
	}

	public async Task<BaseDataTableResponse<List<DetailsCategoriesNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetCategoryFiltersRequest request)
	{
		var result = new BaseDataTableResponse<List<DetailsCategoriesNewsResponseItem>>();

		var query = (from n in statisticsContext.News
					 join s in statisticsContext.NewsStatus on n.StatusID equals s.ID
					 join c in statisticsContext.Categories on n.CategoryID equals c.ID
					 join p in statisticsContext.NewsPhotos on n.ID equals p.NewsID
					 join t in statisticsContext.NewsTypes on n.NewsTypeID equals t.ID
					 join v in statisticsContext.NewsViewsDailies on n.ID equals v.NewsID
					 where p.PhotoTypeID == (int)NewsPhotoTypesEnum.MainPhoto && n.CategoryID == request.ID
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

		result.Data = await query.Select(f => new DetailsCategoriesNewsResponseItem
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

	public async Task<ServiceResult<List<GetCategoryTreeItem>>> GetCategoriesTree(int? parent, bool onlyEnableds)
	{
		var result = new ServiceResult<List<GetCategoryTreeItem>>();

		var categories = await (from c in _context.Categories
								where (parent.HasValue ? c.ParentID == parent : c.ParentID == null)
									  && (!onlyEnableds || c.IsEnabled)
								orderby c.Name
								let childCount = _context.Categories.Count(cc => cc.ParentID == c.ID)
								select new GetCategoryTreeItem
								{
									Id = c.ID,
									text = c.Name,
									icon = c.IsEnabled ? "fa fa-folder text-primary" : "fa fa-folder text-secondary",
									children = childCount > 0,
									type = parent.HasValue ? "root" : "child",
									IsEnabled = c.IsEnabled,
								}).ToListAsync();

		result.Data = categories;

		return result;
	}

	public async Task<ServiceResult> ChangeCategoryStatus(int ID, bool isEnabled)
	{
		var result = new ServiceResult();

		var categories = _context.Categories.Where(x => x.ID == ID).ToList();
		if (!categories.Any())
		{
			result.SetError(_localizer["RecordNotFound"]);
			return result;
		}

		foreach (var category in categories)
		{
			category.IsEnabled = isEnabled;
			_context.Categories.Update(category);
		}

		if (!categories[0].ParentID.HasValue)
		{
			var childCategories = _context.Categories.Where(x => x.ParentID == categories[0].ID).ToList();
			foreach (var category in childCategories)
			{
				category.IsEnabled = isEnabled;
				_context.Categories.Update(category);
			}
		}

		await _context.SaveChangesAsync();

		result.SetSuccess(_localizer["RecordUpdated"]);
		return result;
	}

}