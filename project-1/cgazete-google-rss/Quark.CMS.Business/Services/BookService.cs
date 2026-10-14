using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.FtpUpload;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.BookRequests;
using Quark.CMS.Business.Models.Responses.BookResponses;
using Quark.Data;
using Quark.Data.Models;
using Quark.Data.Models.Common;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class BookService : BaseService, IBookService
{
    private readonly FileUpload _fileUpload;
    private readonly IBookCachingService _bookCachingService;

    public BookService(QuarkDataContext context, IHttpContextAccessor httpContext, IStringLocalizer<CommonResource> localizer, FileUpload fileUpload, AppSettingsKeys appSettingsKeys, ICacheManager cache, IBookCachingService bookCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _fileUpload = fileUpload;
        _bookCachingService = bookCachingService;
    }

    public ServiceResult<GetBookFiltersRequest> GetAllPagedFiltersAsync()
    {
        var result = new ServiceResult<GetBookFiltersRequest>();

        var filters = new GetBookFiltersRequest { IsEnabled = true };

        result.Data = filters;
        return result;
    }

    public async Task<BaseDataTableResponse<List<ListResponseBookItem>>> GetAllPagedAsync(BaseDataTableRequest tableRequest, GetBookFiltersRequest request)
    {
        var result = new BaseDataTableResponse<List<ListResponseBookItem>>();

        string enabledText = _localizer["Enabled"];
        string disabledText = _localizer["Disabled"];

        var query = (from b in _context.Books
                     select new
                     {
                         ID = b.ID,
                         Name = b.Name,
                         Author = b.Author,
                         ImageURL = b.ImageURL.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_BooksImageFolder),
                         IsEnabled = b.IsEnabled,
                         IsEnabledText = b.IsEnabled ? enabledText : disabledText
                     }).AsNoTracking().AsQueryable();

        result.RecordsTotal = query.Count();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(x => x.Name.Contains(tableRequest.SearchValue));
        }

        result.RecordsFiltered = query.Count();

        query = query.SortSkipTake(tableRequest);

        result.Data = await query.Select(x => new ListResponseBookItem
        {
            ID = x.ID,
            Name = x.Name,
            IsEnabled = x.IsEnabled,
            ImageURL = x.ImageURL,
            Author = x.Author,
            IsEnabledText = x.IsEnabledText
        }).ToListAsync();

        return result;
    }

    public async Task<ServiceResult<CreateBookRequest>> Create()
    {
        var result = new ServiceResult<CreateBookRequest>();
        var createBookRequest = new CreateBookRequest
        {
            IsEnabled = true,
            Slug = _appSettingsKeys.SlugPrefix_Books
        };

        result.Data = createBookRequest;
        return result;
    }

    public async Task<ServiceResult<CreateBookRequest>> CreateAsync(CreateBookRequest request)
    {
        var result = new ServiceResult<CreateBookRequest> { Data = request };
        var existedBook = await _context.Books.Where(x => x.Name == request.Name.Trim()).FirstOrDefaultAsync();

        if (existedBook != null)
        {
            result.SetError(_localizer["RecordAlreadyExist"]);
            return result;
        }
        if (request.Image != null)
        {
            var uploadResult = _fileUpload.Upload(request.Image, _appSettingsKeys.CDN_BooksImageFolder);
            if (uploadResult.IsSuccess)
            {
                request.ImageURL = uploadResult.FileName;
            }

        }
        else
        {
            result.SetError(_localizer["ImageUploadRequired"]);
            return result;
        }

        var book = new Books
        {
            ID = request.ID,
            Name = request.Name.Trim(),
            ImageURL = request.ImageURL,
            Author = request.Author,
            Description = request.Description?.Trim(),
            Slug = request.Slug?.Trim(),
            SeoTitle = request.SeoTitle?.Trim(),
            SeoDescription = request.SeoDescription?.Trim(),
            BaseURL = request.BaseURL?.Trim(),
            IsEnabled = true
        };

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        result.Data.ID = book.ID;

        if (request.Slug.Contains("{ID}"))
        {
            book.Slug = request.Slug.Replace("{ID}", book.ID.ToString());

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        await _bookCachingService.RefreshAll();
        result.SetSuccess(_localizer["RecordAdded"]);
        return result;
    }

    public async Task<ServiceResult<UpdateBookRequest>> UpdateAsync(int id)
    {
        var result = new ServiceResult<UpdateBookRequest>();

        var book = await _context.Books.FirstOrDefaultAsync(b => b.ID == id);
        if (book == null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        var updateBookRequest = new UpdateBookRequest
        {
            ID = book.ID,
            Name = book.Name,
            Author = book.Author,
            IsEnabled = book.IsEnabled,
            Description = book.Description,
            ImageURL = book.ImageURL.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_BooksImageFolder),
            Slug = book.Slug,
            SeoTitle = book.SeoTitle,
            SeoDescription = book.SeoDescription
        };

        result.Data = updateBookRequest;
        return result;
    }

    public async Task<ServiceResult<UpdateBookRequest>> UpdateAsync(UpdateBookRequest request)
    {
        var result = new ServiceResult<UpdateBookRequest> { Data = request };

        var book = await _context.Books.FirstOrDefaultAsync(b => b.ID == request.ID);
        if (book is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        if (request.Image != null)
        {
            var uploadResult = _fileUpload.Upload(request.Image, _appSettingsKeys.CDN_BooksImageFolder);
            if (uploadResult.IsSuccess)
            {
                book.ImageURL = uploadResult.FileName;
            }
        }
        else
        {
            if (book.ImageURL == null)
            {
                result.SetError(_localizer["ImageUploadRequired"]);
                return result;
            }
        }

        if (request.Slug.Contains("{ID}"))
        {
            request.Slug = request.Slug.Replace("{ID}", book.ID.ToString());
        }

        if (!string.IsNullOrWhiteSpace(book.Slug) &&
        !string.Equals(book.Slug, request.Slug, StringComparison.OrdinalIgnoreCase))
        {
			var urlMapping = await _context.UrlMappings
		.Where(x => x.NewSlug == book.Slug && x.EntityTypeID == (int)EntityTypeEnum.Books).ToListAsync();

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
                OldSlug = book.Slug,
                NewSlug = request.Slug,
                EntityTypeID = (int)EntityTypeEnum.Books,
                CreatedDate = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        book.Slug = request.Slug;
        book.ID = request.ID;
        book.Name = request.Name;
        book.Author = request.Author;
        book.Description = request.Description;
        book.SeoTitle = request.SeoTitle;
        book.SeoDescription = request.SeoDescription;

        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        await _bookCachingService.RefreshAll();
        result.SetSuccess(_localizer["RecordUpdated"]);
        return result;
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var result = new ServiceResult();

        var book = await _context.Books.FirstOrDefaultAsync(b => b.ID == id);
        if (book is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        book.IsEnabled = false;
        _context.Books.Update(book);

        var urlMapping = await _context.UrlMappings.Where(x => x.NewSlug == book.Slug && x.EntityTypeID == (int)EntityTypeEnum.Books).ToListAsync();

        if (urlMapping.Count > 0)
        {
            _context.UrlMappings.RemoveRange(urlMapping);
        }

        await _context.SaveChangesAsync();
        await _bookCachingService.GetAllBooks();
        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }

    public async Task<ServiceResult<BookDetailsResponse>> DetailsAsync(int id)
    {
        var result = new ServiceResult<BookDetailsResponse>();

        result.Data = await (from a in _context.Books
                             where a.ID == id
                             select new BookDetailsResponse

                             {
                                 ID = a.ID,
                                 Name = a.Name,
                                 Author = a.Author,
                                 Description = a.Description,
                                 Slug = a.Slug,
                                 SeoTitle = a.SeoTitle,
                                 ImageURL = a.ImageURL.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_BooksImageFolder),
                                 SeoDescription = a.SeoDescription,
                                 IsEnabled = a.IsEnabled,
                             }).FirstOrDefaultAsync();
        return result;
    }
}
