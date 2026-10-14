using AvvaMobile.Core.Business;
using Quark.API.Business.Models.Responses.BookResponses;
using Quark.Business.Caching.Models.Responses.BookCachingResponses;

namespace Quark.API.Business.Interfaces
{
    public interface IBookService
    {
        Task<ServiceResult<GetBookResponse>> GetById(int id);
        Task<ServiceResult<GetBookResponse>> GetByName(string name);
        Task<ServiceResult<List<BookCachingResponse>>> GetAllBooks();

    }
}