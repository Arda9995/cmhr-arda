using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Requests.BookCachingRequests;
using Quark.Business.Caching.Models.Responses.BookCachingResponses;

namespace Quark.Business.Caching.Interfaces;
public interface IBookCachingService
{
    Task<ServiceResult<BookCachingResponse>> GetBookById(int id);
    Task<ServiceResult<BookCachingResponse>> GetBookByName(string name);
    Task<ServiceResult<List<BookCachingResponse>>> GetAllBooks();
    Task<ServiceResult> RefreshAll();


}
