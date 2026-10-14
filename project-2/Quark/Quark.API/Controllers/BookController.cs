using AvvaMobile.Core.APIControllers;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;

namespace Quark.API.Controllers
{
    public class BookController(IBookService bookService) : ApiBaseController
    {
        [HttpPost]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await bookService.GetByName(name);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await bookService.GetById(id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> GetAllBooks()
        {
            var result = await bookService.GetAllBooks();
            return Ok(result);
        }
        
    }
}
