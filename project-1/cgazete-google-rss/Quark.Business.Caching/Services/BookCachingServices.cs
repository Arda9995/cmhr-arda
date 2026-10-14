using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.BookCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Business.Common.Caching;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.Business.Caching.Services;

public class BookCachingService : BaseService, IBookCachingService
{
    public BookCachingService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache)
        : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
    }

    public async Task<ServiceResult> RefreshAll()
    {
        var result = new ServiceResult();

        var books = await _context.Books.Where(x=>x.IsEnabled)
            .Select(x => new BookCachingResponse
        {
            ID = x.ID,
            Name = x.Name,
            Author = x.Author,
            IsEnabled = x.IsEnabled,
            BaseURL = x.BaseURL,
            ImageURL = x.ImageURL.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_BooksImageFolder),
            Description = x.Description,
            Slug = x.Slug,
            SeoTitle = x.SeoTitle,
            SeoDescription = x.SeoDescription,
        }).ToListAsync();
        await _cache.Set(RedisKeys.Books, books);
        return result;
    }

    public async Task<ServiceResult<BookCachingResponse>> GetBookById(int id)
    {
        var result = new ServiceResult<BookCachingResponse>();

        var books = await _cache.Get<List<BookCachingResponse>>(RedisKeys.Books);

        if (books == null)
        {
            await RefreshAll();
            books = await _cache.Get<List<BookCachingResponse>>(RedisKeys.Books);
        }

        result.Data = books.Where(x => x.ID == id).FirstOrDefault();

        return result;
    }

    public async Task<ServiceResult<BookCachingResponse>> GetBookByName(string name)
    {
        var result = new ServiceResult<BookCachingResponse>();

        if (name != null)
        {
            var books = await _cache.Get<List<BookCachingResponse>>(RedisKeys.Books);

            if (books == null)
            {
                await RefreshAll();
                books = await _cache.Get<List<BookCachingResponse>>(RedisKeys.Books);
            }

            result.Data = books.Where(x => x.Name.Contains(name)).FirstOrDefault();
        }
        return result;
    }

    public async Task<ServiceResult<List<BookCachingResponse>>> GetAllBooks()
    {
        var result = new ServiceResult<List<BookCachingResponse>>();

        var cachedBooks = await _cache.Get<List<BookCachingResponse>>(RedisKeys.Books);

        if (cachedBooks == null)
        {
            await RefreshAll();
            cachedBooks = await _cache.Get<List<BookCachingResponse>>(RedisKeys.Books);
        }

        result.Data = cachedBooks.Where(x=>x.IsEnabled).ToList();
        return result;
    }
}
