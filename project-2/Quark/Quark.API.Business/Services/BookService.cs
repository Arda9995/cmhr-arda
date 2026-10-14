using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.BookResponses;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Caching.Models.Responses.BookCachingResponses;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;

namespace Quark.API.Business.Services
{
    public class BookService : BaseService, IBookService
    {
        private readonly IBookCachingService _bookCachingService;
        public BookService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, Quark.Business.Common.Caching.AppSettingsKeys appSettingsKeys, ICacheManager cache, IBookCachingService bookCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
        {
            _bookCachingService = bookCachingService;
        }

        public async Task<ServiceResult<GetBookResponse>> GetById(int id)
        {
            var result = new ServiceResult<GetBookResponse>();

            var book = await _bookCachingService.GetBookById(id);

            if (book != null)
            {
                var bookResult = new GetBookResponse
                {
                    ID = book.Data.ID,
                    Name = book.Data.Name,
                    Author = book.Data.Author,
                    IsEnabled = book.Data.IsEnabled,
                    BaseURL = book.Data.BaseURL,
                    ImageURL = book.Data.ImageURL,
                    Description = book.Data.Description,
                    Slug = book.Data.Slug,
                    SeoTitle = book.Data.SeoTitle,
                    SeoDescription = book.Data.SeoDescription
                };

                result.Data = bookResult;
            }
            return result;
        }

        public async Task<ServiceResult<GetBookResponse>> GetByName(string name)
        {
            var result = new ServiceResult<GetBookResponse>();

            var book = await _bookCachingService.GetBookByName(name);

            if (book != null)
            {
                var bookResult = new GetBookResponse
                {
                    ID = book.Data.ID,
                    Name = book.Data.Name,
                    Author = book.Data.Author,
                    IsEnabled = book.Data.IsEnabled,
                    BaseURL = book.Data.BaseURL,
                    ImageURL = book.Data.ImageURL,
                    Description = book.Data.Description,
                    Slug = book.Data.Slug,
                    SeoTitle = book.Data.SeoTitle,
                    SeoDescription = book.Data.SeoDescription
                };

                result.Data = bookResult;
            }
            return result;
        }

        public async Task<ServiceResult<List<BookCachingResponse>>> GetAllBooks()
        {
            var result = new ServiceResult<List<BookCachingResponse>>();

            var books = await _bookCachingService.GetAllBooks();

            if (books?.Data != null)
            {
                result.Data = books.Data;
            }

            return result;
        }
    }
}
