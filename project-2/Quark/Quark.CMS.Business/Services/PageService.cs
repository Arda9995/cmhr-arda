using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.Cloudflare.Adapter.Interfaces;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.PageRequests;
using Quark.CMS.Business.Models.Responses.PageResponses;
using Quark.Core.Extentions;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.Common;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class PageService : BaseService, IPageService
{
    private readonly IPageCachingService _pageCachingService;
    private readonly IUrlMappingService _urlMappingService;
    private readonly ICloudflareAdapter _cloudflareAdapter;

    public PageService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IPageCachingService pageCachingService, IUrlMappingService urlMappingService, ICloudflareAdapter cloudflareAdapter) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _urlMappingService = urlMappingService;
        _pageCachingService = pageCachingService;
        _context = context;
        _cloudflareAdapter = cloudflareAdapter;
    }

    public async Task<ServiceResult<List<SelectListItem>>> GetAllSelectList()
    {
        var result = new ServiceResult<List<SelectListItem>>();

        result.Data = (await _pageCachingService.PagesAsSelectListItems()).Data;

        result.Data.AddSELECTOption();

        return result;
    }

    public ServiceResult<CreateRequest> Create()
    {
        var result = new ServiceResult<CreateRequest>();
        var createPage = new CreateRequest();


        result.Data = createPage;
        return result;
    }

    public async Task<ServiceResult<CreateRequest>> CreateAsync(CreateRequest request)
    {
        var result = new ServiceResult<CreateRequest> { Data = request };

        var isPageExist = await _context.Pages.Where(x => x.Key == request.Key.Trim()).AnyAsync();

        if (isPageExist)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        if (!request.Keywords.IsNullOrEmtpy())
        {
            var seoWords = request.Keywords.Replace("[{\"value\":\"", "").Replace("\"},{\"value\":\"", ",").Replace("\"}]", "");

            request.Keywords = string.Join(',', seoWords.Split(",").Select(x => x.ToKeyword()));
        }

        var page = new Page
        {
            Name = request.Name.Trim(),
            Key = request.Key,
            Keywords = request.Keywords,
            Content = request.Content,
            Description = request.Description,
            Slug = request.Slug,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            Breadcrumb = request.Breadcrumb
        };

        await _context.Pages.AddAsync(page);
        await _context.SaveChangesAsync();
		await _pageCachingService.RefreshAll();
        await _pageCachingService.RefreshPagesSlugs();

        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var result = new ServiceResult();

        var page = await _context.Pages.FirstOrDefaultAsync(x => x.ID == id);

        if (page == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }
        _context.Pages.Remove(page);

        var urlMapping = await _context.UrlMappings.Where(x => x.NewSlug == page.Slug && x.EntityTypeID == (int)EntityTypeEnum.Pages).ToListAsync();

        if (urlMapping.Count > 0)
        {
            _context.UrlMappings.RemoveRange(urlMapping);
        }
        await _context.SaveChangesAsync();
		await _urlMappingService.NotifyUrlMappingChanged();
		await _pageCachingService.RefreshAll();
        await _pageCachingService.RefreshPagesSlugs();


        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest)
    {
        var result = new BaseDataTableResponse<List<ListResponseItem>>();

        result.RecordsTotal = _context.Pages.Count();

        var query = (from p in _context.Pages
                     orderby p.Name
                     select new
                     {
                         ID = p.ID,
                         Name = p.Name,
                         Description = p.Description,
                         Key = p.Key
                     }).AsNoTracking().AsQueryable();

        if (tableRequest.SearchAllColumns && !string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x =>
                x.Name.ToLower().Contains(tableRequest.SearchValue.ToLower()));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseItem
        {
            ID = x.ID,
            Name = x.Name,
            Description = x.Description.ToEmptyStringIndicator(),
            Key = x.Key
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<UpdateRequest>> UpdateAsync(int id)
    {
        var result = new ServiceResult<UpdateRequest>();

        var page = await (from p in _context.Pages
                          where p.ID == id
                          select new UpdateRequest
                          {
                              ID = p.ID,
                              Name = p.Name,
                              Content = p.Content,
                              Description = p.Description,
                              Keywords = p.Keywords,
                              Key = p.Key,
                              SeoTitle = p.SeoTitle,
                              SeoDescription = p.SeoDescription,
                              Breadcrumb = p.Breadcrumb,
                              Slug = p.Slug,
                          }).FirstOrDefaultAsync();

        if (page is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        result.Data = page;
        return result;
    }

    public async Task<ServiceResult<UpdateRequest>> UpdateAsync(UpdateRequest request)
    {
        var result = new ServiceResult<UpdateRequest> { Data = request };

        var page = await _context.Pages.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (page is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var isPageExist = await _context.Pages.Where(x => x.Key == request.Key.Trim() && x.ID != request.ID).AnyAsync();

        if (isPageExist)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }

        if (!request.Keywords.IsNullOrEmtpy())
        {
            var seoWords = request.Keywords.Replace("[{\"value\":\"", "").Replace("\"},{\"value\":\"", ",").Replace("\"}]", "");

            page.Keywords = string.Join(',', seoWords.Split(",").Select(x => x.ToKeyword()));
        }


        if (!string.IsNullOrWhiteSpace(page.Slug) &&
    !string.Equals(page.Slug, request.Slug, StringComparison.OrdinalIgnoreCase))
        {
			var urlMapping = await _context.UrlMappings
		.Where(x => x.NewSlug == page.Slug && x.EntityTypeID == (int)EntityTypeEnum.Pages).ToListAsync();

			if (urlMapping != null)
			{
				foreach (var url in urlMapping)
				{
					if (url.OldSlug == request.Slug)
					{
						_context.UrlMappings.Remove(url);
					}
					else
					{
						url.NewSlug = request.Slug;
						_context.UrlMappings.Update(url);
					}
				}
			}
			await _context.UrlMappings.AddAsync(new UrlMapping
            {
                NewSlug = request.Slug,
                OldSlug = page.Slug,
                EntityTypeID = (int)EntityTypeEnum.Pages,
                CreatedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
			await _urlMappingService.NotifyUrlMappingChanged();
		}

        page.Keywords = request.Keywords;
        page.Name = request.Name;
        page.Content = request.Content;
        page.Description = request.Description;
        page.Key = request.Key;
        page.Slug = request.Slug;
        page.SeoDescription = request.SeoDescription;
        page.Breadcrumb = request.Breadcrumb;

        _context.Pages.Update(page);
        await _context.SaveChangesAsync();


        await _pageCachingService.RefreshAll();
        await _pageCachingService.RefreshPagesSlugs();
        _cloudflareAdapter.Purge(request.Slug.TrimStart('/').PrepareWebsiteUrl(_appSettingsKeys.Common_ClientWebSite));

        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }
}