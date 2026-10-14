using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.MediaGallery;

namespace Quark.CMS.Controllers;

[Authorize]
public class MediaGalleryController : BaseController
{

    private readonly IMediaGalleryService _mediaGalleryService;

    public MediaGalleryController(IMediaGalleryService mediaGalleryService)
    {
        _mediaGalleryService = mediaGalleryService;
    }


    public IActionResult List()
    {
        return View();
    }

    public async Task<IActionResult> GetAllFolders()
    {
        var result = await _mediaGalleryService.GetAllFoldersAsync(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);
    }
    
    public async Task<IActionResult> GetTransferFolders()
    {
        var result = await _mediaGalleryService.GetTransferFolders();
        return Json(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> TransferPhoto(TransferPhotoRequest request)
    {
        var result = await _mediaGalleryService.TransferPhoto(request);
        return Json(result);
    }
    
    public async Task<IActionResult> GetAllArchiveFolders()
    {
        var result = await _mediaGalleryService.GetAllArchiveFoldersAsync(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddFolder(AddFolderRequest request)
    {
        var result = await _mediaGalleryService.AddFolderAsync(request);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> FolderImages(GetImagesFilterRequest request)
    {
        var result = await _mediaGalleryService.GetFolderImagesAsync(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> ArchiveFolderImages(GetImagesFilterRequest request)
    {
        var result = await _mediaGalleryService.GetArchiveFolderImagesAsync(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> FavouriteImages(GetImagesFilterRequest request)
    {
        var result = await _mediaGalleryService.GetFavouritesAsync(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> GetFilePath(List<int> foldersIds, int currentID)
    {
        var result = await _mediaGalleryService.GetFilePathAsync(foldersIds, currentID);
        return View("_FilePath", result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(UploadImageRequest request)
    {
        var response = await _mediaGalleryService.UploadImageAsync(request);
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFolder(int folderId)
    {
        var response = await _mediaGalleryService.DeleteFolderAsync(folderId);
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteImage(int id)
    {
        var response = await _mediaGalleryService.DeleteImageAsync(id);
        return Json(response);
    }
    [HttpPost]
    public async Task<IActionResult> AddFavourite(int imageId)
    {
        var response = await _mediaGalleryService.AddFavourite(imageId);
        return Json(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteFavourite(int imageId)
    {
        var response = await _mediaGalleryService.DeleteFavourite(imageId);
        return Json(response);
    }

    [HttpGet]
    public async Task<JsonResult> GetGalleryTree(int? parent)
    {
        var result = await _mediaGalleryService.GetGalleryTreeAsync(parent);
        return Json(result.Data);
    }
    
    [HttpGet]
    public async Task<JsonResult> GetArchiveTree(int? parent)
    {
        var result = await _mediaGalleryService.GetArchiveTree(parent);
        return Json(result.Data);
    }    
    
    [HttpPost]
    public async Task<JsonResult> ChangeImageName(ChangeImageNameRequest request)
    {
        var result = await _mediaGalleryService.ChangeImageNameAsync(request);
        return Json(result);
    }
}