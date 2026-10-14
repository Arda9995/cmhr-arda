using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.BookRequests;

namespace Quark.CMS.Controllers;

public class BooksController : BaseController
{
    private readonly IBookService _service;
    public BooksController(IBookService bookService)
    { _service = bookService; }

    public IActionResult List()
    {
        var result = _service.GetAllPagedFiltersAsync();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPagedAsync(GetBookFiltersRequest request)
    {
        var result = await _service.GetAllPagedAsync(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }

    public async Task<IActionResult> Create()
    {
        var result = await _service.Create();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookRequest request)
    {
        var result = await _service.CreateAsync(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Update(int id)
    {
        var result = await _service.UpdateAsync(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction(nameof(List));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateBookRequest request)
    {
        var result = await _service.UpdateAsync(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect("/Books/Details/" + request.ID);

    }

    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return Json(result);
    }

    public async Task<IActionResult> Details(int id)
    {
        var result = await _service.DetailsAsync(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction(nameof(List));
        }
        return View(result.Data);
    }
}