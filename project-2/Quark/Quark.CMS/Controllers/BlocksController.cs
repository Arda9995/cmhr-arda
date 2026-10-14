using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.BlockRequests;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.BLOCKS)]
public class BlocksController : BaseController
{
    private readonly IBlockService _blockService;

    public BlocksController(IBlockService blockService)
    {
        _blockService = blockService;
    }

    public async Task<IActionResult> List()
    {
        var result = await _blockService.GetAllPagedFilters();
        
        return View(result.Data);    
    }
    
    [HttpPost]
    public async Task<IActionResult> GetAllPaged(ListFiltersRequest request)
    {
        var result = await _blockService.GetAllPaged(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> GetNewsPaged(GetBlockNewsFiltersRequest request)
    {
        var result = await _blockService.GetNewsPaged(request);

        return PartialView("_NewsList", result);
    }

    public async Task<IActionResult> GetAllSelectList(int selectedPageId)
    {
        var result = await _blockService.GetAllSelectList(selectedPageId);

        return Ok(result);
    }

    public async Task<IActionResult> GetMinMaxValues(int blockId)
    {
        var result = await _blockService.GetMinMaxValues(blockId);

        return Ok(result);
    }
    
    public async Task<IActionResult> Create()
    {
        var result = await _blockService.Create();
        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateUpdateRequest request)
    {
        var result = await _blockService.Create(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return RedirectToAction(nameof(List));
    }
    
    public async Task<IActionResult> Update(int id)
    {
        var result = await _blockService.Update(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction(nameof(List));
        }
        return View(result.Data);
    }
        
    [HttpPost]
    public async Task<IActionResult> Update(CreateUpdateRequest request)
    {
        var result = await _blockService.Update(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }
        return RedirectToAction(nameof(List));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _blockService.Delete(id);
        return Json(result);
    }

    public async Task<IActionResult> SaveBlockNews(CreateBlockNewsRequest request)
    {
        var result = await _blockService.SaveBlockNews(request);
        return  Json(result);
    }

    public async Task<IActionResult> GetNewsByBlockId(int blockId) 
    { 
        var result = await _blockService.GetNewsByBlockId(blockId); 
        return PartialView("_NewsItems", result.Data); 
    } 
 
    public IActionResult Manage() 
    { 
        return View(); 
    } 
}