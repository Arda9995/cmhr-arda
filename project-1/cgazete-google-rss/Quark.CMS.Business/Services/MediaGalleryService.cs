using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using AvvaMobile.Core.Utilities.FtpUpload;
using BunnyCDN.Net.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.MediaGallery;
using Quark.CMS.Business.Models.Responses.MediaGalleryResponses;
using Quark.Data;
using Quark.Data.Models.MediaGallery;
using Quark.Resources;
using SixLabors.ImageSharp;
using System.IO;
using System.Linq.Dynamic.Core;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class MediaGalleryService : BaseService, IMediaGalleryService
{
    private const int MaxPageSize = 100;
    private const int RootFolderCountLimit = 10000;

    private readonly FileUpload _fileUpload;
    public MediaGalleryService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, FileUpload fileUpload) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _fileUpload = fileUpload;
    }

    public async Task<BaseDataTableResponse<List<GetAllFoldersResponse>>> GetAllFoldersAsync(BaseDataTableRequest tableRequest)
    {
        var response = new BaseDataTableResponse<List<GetAllFoldersResponse>>();

        var foldersQuery = (from permissionFolder in _context.FolderPermissions
                            join folder in _context.Folders on permissionFolder.FolderID equals folder.ID
                            join user in _context.Users on folder.CreatedBy equals user.ID
                            where permissionFolder.UserID == CurrentUserID
                            select new
                            {
                                Id = folder.ID,
                                Name = folder.Name,
                                CreatedUserName = user.Firstname + " " + user.Lastname,
                                CreatedDateTime = folder.CreatedDateTime.ToString("dd.MM.yyyy HH:mm")
                            }).AsQueryable();

        response.RecordsTotal = foldersQuery.Count();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            foldersQuery = foldersQuery.Where(
                x => x.Name.ToUpper().Contains(tableRequest.SearchValue.ToUpper()) ||
                     x.CreatedUserName.ToUpper().Contains(tableRequest.SearchValue.ToUpper())
            );
        }

        if (tableRequest.PageSize > 100)
        {
            tableRequest.PageSize = 100;
        }


        var folders = await foldersQuery.Skip(tableRequest.Skip).Take(tableRequest.PageSize)
            .OrderBy(x => x.Name)
            .Select(x => new GetAllFoldersResponse
            {
                Id = x.Id,
                Name = x.Name,
                CreatedUserName = x.CreatedUserName,
                CreatedDateTime = x.CreatedDateTime
            }).ToListAsync();


        response.Data = folders;
        return response;
    }

    public async Task<BaseDataTableResponse<List<FolderImageInfoResponse>>> GetFolderImagesAsync(BaseDataTableRequest tableRequest, GetImagesFilterRequest request)
    {
        var result = new BaseDataTableResponse<List<FolderImageInfoResponse>>();

        var folder = await _context.Folders.FirstOrDefaultAsync(x => x.ID == request.FolderId);

        IQueryable<FolderImageInfoResponse> query;

        if (folder != null && folder.ParentID == -1)
        {

            query = (from image in _context.FolderImages
                     join user in _context.Users on image.UploadedBy equals user.ID
                     let isFav = _context.Favourites.Any(x => x.ImageID == image.ID && x.UserID == CurrentUserID)
                     orderby image.UploadedDate descending
                     select new FolderImageInfoResponse
                     {
                         Id = image.ID,
                         ImageUrl = image.ImageURL,
                         FileName = image.ImageName,
                         UploadDate = image.UploadedDate,
                         FormattedUploadDate = image.UploadedDate.ToString("G"),
                         UploadedBy = user.Firstname + " " + user.Lastname,
                         IsFavourite = isFav
                     }).AsNoTracking().AsQueryable();
        }
        else
        {

            query = (from image in _context.FolderImages
                     join user in _context.Users on image.UploadedBy equals user.ID
                     let isFav = _context.Favourites.Any(x => x.ImageID == image.ID && x.UserID == CurrentUserID)
                     where image.FolderID == request.FolderId
                     orderby image.UploadedDate descending
                     select new FolderImageInfoResponse
                     {
                         Id = image.ID,
                         ImageUrl = image.ImageURL,
                         FileName = image.ImageName,
                         UploadDate = image.UploadedDate,
                         FormattedUploadDate = image.UploadedDate.ToString("G"),
                         UploadedBy = user.Firstname + " " + user.Lastname,
                         IsFavourite = isFav
                     }).AsNoTracking().AsQueryable();
        }

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(x => x.FileName.Contains(request.SearchValue));
        }

        if (tableRequest.PageSize <= 0 || tableRequest.PageSize > MaxPageSize)
        {
            tableRequest.PageSize = MaxPageSize;
        }

        var isRootFolder = folder != null && folder.ParentID == -1;
        var recordsTotal = isRootFolder
            ? await query.Select(x => x.Id).Take(RootFolderCountLimit).CountAsync()
            : await query.Select(x => x.Id).CountAsync();

        var data = await query.SortSkipTake(tableRequest).ToListAsync();

        result.RecordsTotal = recordsTotal;
        result.RecordsFiltered = recordsTotal;
        result.Data = data;

        return result;
    }

    public async Task<BaseDataTableResponse<List<GetAllFoldersResponse>>> GetAllArchiveFoldersAsync(BaseDataTableRequest tableRequest)
    {
        var response = new BaseDataTableResponse<List<GetAllFoldersResponse>>();

        var foldersQuery = (from f in _context.ArchiveFolders
                            select new
                            {
                                Id = f.ID,
                                Name = f.Name
                            }).AsQueryable();

        response.RecordsTotal = foldersQuery.Count();

        if (tableRequest.PageSize > 100)
        {
            tableRequest.PageSize = 100;
        }

        var folders = await foldersQuery.Skip(tableRequest.Skip).Take(tableRequest.PageSize)
            .OrderBy(x => x.Name)
            .Select(x => new GetAllFoldersResponse
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();


        response.Data = folders;
        return response;
    }

    public async Task<BaseDataTableResponse<List<FolderImageInfoResponse>>> GetArchiveFolderImagesAsync(BaseDataTableRequest tableRequest, GetImagesFilterRequest request)
    {
        var result = new BaseDataTableResponse<List<FolderImageInfoResponse>>();

        var query = from image in _context.ArchiveImages
                    join user in _context.Users on image.UploadedBy equals user.ID
                    where image.FolderID == request.FolderId
                    select new FolderImageInfoResponse
                    {
                        Id = image.ID,
                        ImageUrl = image.ImageURL.PrepareCDNUrl(_appSettingsKeys.CDN_BaseUrl, _appSettingsKeys.CDN_MediaGalleryFolder),
                        UploadDate = image.UploadedDate,
                        FormattedUploadDate = image.UploadedDate.ToString("G"),
                        UploadedBy = user.FullName,
                        FileName = image.ImageURL
                    };

        result.RecordsTotal = await query.CountAsync();

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(x => x.FileName.Contains(request.SearchValue));
            result.RecordsFiltered = await query.CountAsync();
        }
        else
        {
            result.RecordsFiltered = result.RecordsTotal;
        }

        if (tableRequest.PageSize <= 0 || tableRequest.PageSize > MaxPageSize)
        {
            tableRequest.PageSize = MaxPageSize;
        }

        query = query.SortSkipTake(tableRequest);

        var data = await query.ToListAsync();

        if (request.FolderId != 0)
        {
            foreach (var item in data)
            {
                var str = request.FolderId.ToString();
                item.Tooltip = $"{str.Substring(0, 4)}/{str.Substring(4, 2)}/{str.Substring(6, 2)}/{item.FileName}";
            }
        }

        result.Data = data;
        return result;
    }

    public async Task<BaseDataTableResponse<List<FolderImageInfoResponse>>> GetFavouritesAsync(BaseDataTableRequest tableRequest, GetImagesFilterRequest request)
    {
        var result = new BaseDataTableResponse<List<FolderImageInfoResponse>>();

        var query = (from f in _context.Favourites
                     join i in _context.FolderImages on f.ImageID equals i.ID
                     join u in _context.Users on f.UserID equals u.ID
                     where f.UserID == CurrentUserID
                     select new FolderImageInfoResponse
                     {
                         Id = f.ID,
                         ImageUrl = i.ImageURL,
                         FileName = i.ImageName,
                         FolderID = i.FolderID,
                         ImageID = f.ImageID,
                     }).AsNoTracking().AsQueryable();

        result.RecordsTotal = await query.CountAsync();

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            query = query.Where(x => x.FileName.Contains(request.SearchValue));
            result.RecordsFiltered = await query.CountAsync();
        }
        else
        {
            result.RecordsFiltered = result.RecordsTotal;
        }

        if (tableRequest.PageSize <= 0 || tableRequest.PageSize > MaxPageSize)
        {
            tableRequest.PageSize = MaxPageSize;
        }

        query = query.SortSkipTake(tableRequest);

        var data = await query.ToListAsync();

        var folders = await _context.Folders.AsNoTracking().ToListAsync();

        foreach (var img in data)
        {
            var currentFolder = folders.FirstOrDefault(x => x.ID == img.FolderID);
            if(currentFolder != null)
            {
                img.Tooltip = currentFolder.Name + "/" + img.FileName;
                while (currentFolder.ParentID != 0 && currentFolder.ParentID != -1)
                {
                    currentFolder = folders.FirstOrDefault(x => x.ID == currentFolder.ParentID);
                    img.Tooltip = currentFolder.Name + "/" + img.Tooltip;
                }
            }
         
        }

        result.Data = data;

        return result;
    }

    public async Task<ServiceResult> AddFavourite(int imageId)
    {
        var result = new ServiceResult();

        var isExist =
            await _context.Favourites.AnyAsync(x => x.UserID == CurrentUserID && x.ImageID == imageId);

        if (isExist)
        {
            result.SetError("RecordAlreadyExists");
            return result;
        }

        var favourite = new Favourite
        {
            ImageID = imageId,
            UserID = CurrentUserID
        };

        await _context.Favourites.AddAsync(favourite);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<ServiceResult> DeleteFavourite(int imageId)
    {
        var result = new ServiceResult();

        var favourite =
            await _context.Favourites.FirstOrDefaultAsync(x => x.ImageID == imageId && x.UserID == CurrentUserID);

        if (favourite is null)
        {
            result.SetError("RecordNotFound");
            return result;
        }

        _context.Favourites.Remove(favourite);
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<ServiceResult> AddFolderAsync(AddFolderRequest request)
    {
        var response = new ServiceResult();

        var isExist = _context.Folders.Any(x => x.Name == request.Name && x.ParentID == request.ParentId);
        if (isExist)
        {
            response.SetError(_localizer["RecordAlreadyExist"]);
            return response;
        }

        var folder = new Folder
        {
            Name = request.Name,
            ParentID = request.ParentId,
            CreatedBy = CurrentUserID,
            CreatedDateTime = DateTime.Now
        };

        await _context.Folders.AddAsync(folder);
        await _context.SaveChangesAsync();
        return response;
    }

    public Task<ServiceResult<UpdateFolderRequest>> UpdateFolderAsync(int folderId)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult> UpdateFolderAsync(UpdateFolderRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult> UploadImageAsync(UploadImageRequest request)
    {
        var response = new ServiceResult();

        if (request == null || request.Image == null || request.Image.Length == 0)
        {
            response.SetError(_localizer["Error"]);
            return response;
        }
        var uploadResult = new { FileName = string.Empty, FullPath = string.Empty };
        /*var uploadResult = _fileUpload.Upload(request.Image, _appSettingsKeys.CDN_MediaGalleryFolder);
        if (uploadResult == null)
        {
            response.SetError(_localizer["Error"]);
            return response;
        }*/

        // Benzersiz dosya adý oluþtur
        var extension = Path.GetExtension(request.Image.FileName);
        var uniqueName = Guid.NewGuid().ToString() + extension;

        try
        {
            await using var ms = request.Image.OpenReadStream();

            // BunnyCDN Storage Client
            var storage = new BunnyCDNStorage(
                "media-archive",        // Storage Zone adý
                "255d23b5-6ba0-480d-80e73863d70d-9b1f-48b6",            // API key
                "de"                    // Region (örnek: de, ny, la, sg)
            );

            // Archive dizinine yükle
            var remotePath = $"/media-archive/Archive/{uniqueName}";
            await storage.UploadAsync(ms, remotePath);

            // uploadResult mantýðýný burada manuel oluþturuyoruz
            uploadResult = new
            {
                FileName = uniqueName,
                FullPath = remotePath
            };

            // devamýnda senin mevcut mantýðýn çalýþýr
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"MediaGallery upload failed. File: {request.Image.FileName}, Error: {ex}");
            response.SetError(_localizer["Error"]);
            return response;
        }

        if (string.IsNullOrEmpty(uploadResult.FileName))
        {
            response.SetError(_localizer["Error"]);
            return response;
        }

        var photo = new FolderImage
        {
            FolderID = request.FolderId,
            ImageURL = _appSettingsKeys.CDN_BaseUrl + _appSettingsKeys.CDN_MediaGalleryFolder + uploadResult.FileName,
            ImageName = request.Image.FileName,
            UploadedBy = CurrentUserID,
            UploadedDate = DateTime.Now
        };
        try
        {
            await _context.FolderImages.AddAsync(photo);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.TraceError($"MediaGallery DB save failed. Uploaded CDN file: {uploadResult.FullPath}, Error: {ex}");
            response.SetError(_localizer["Error"]);
            return response;
        }

        response.IsSuccess = true;
        return response;
    }

    public async Task<ServiceResult> DeleteFolderAsync(int folderId)
    {
        var response = new ServiceResult();

        var folder = await _context.Folders.FirstOrDefaultAsync(x => x.ID == folderId);

        if (folder == null)
        {
            response.SetError(_localizer["RecordNotFound"]);
            return response;
        }

        var deleteFolders = await GetFolderWithChildFolders(folder);
        _context.Folders.RemoveRange(deleteFolders);
        _context.FolderImages.RemoveRange(_context.FolderImages.Where(x => deleteFolders.Select(f => f.ID).Contains(x.FolderID)));
        await _context.SaveChangesAsync();
        return response;
    }

    public async Task<ServiceResult> DeleteImageAsync(int imageId)
    {
        var response = new ServiceResult();

        var image = await _context.FolderImages.FirstOrDefaultAsync(x => x.ID == imageId);
        if (image == null)
        {
            response.SetError(_localizer["RecordNotFound"]);
            return response;
        }

        _context.FolderImages.Remove(image);
        await _context.SaveChangesAsync();
        return response;
    }

    public async Task<ServiceResult<List<GetFolderTreeItemResponse>>> GetGalleryTreeAsync(int? parentId)
    {
        var response = new ServiceResult<List<GetFolderTreeItemResponse>>();

        var folders = await _context.Folders
                         .Where(x => parentId.HasValue
                             ? x.ParentID == parentId
                             : x.ParentID == 0 || x.ParentID == -1)
                         .OrderBy(x => x.ParentID == -1 ? 0 : 1)
                         .ThenBy(x => x.Name)
                         .Select(x => new GetFolderTreeItemResponse
                         {
                             Id = x.ID,
                             text = x.Name,
                             icon = "fa fa-folder text-primary",
                             children = _context.Folders.Any(fc => fc.ParentID == x.ID),
                             type = parentId.HasValue ? "child" : "root"
                         })
                         .ToListAsync();



        response.Data = folders;
        return response;
    }

    public async Task<ServiceResult<List<GetFolderTreeItemResponse>>> GetArchiveTree(int? parentId)
    {
        var response = new ServiceResult<List<GetFolderTreeItemResponse>>();

        var folders = await (from a in _context.ArchiveFolders
                             where (parentId.HasValue ? a.ParentID == parentId : a.ParentID == 0)
                             orderby a.Name
                             let childCount = _context.ArchiveFolders.Count(fc => fc.ParentID == a.ID)
                             select new GetFolderTreeItemResponse
                             {
                                 Id = a.ID,
                                 text = a.Name,
                                 icon = "fa fa-folder text-primary",
                                 children = childCount > 0,
                                 type = parentId.HasValue ? "root" : "child"
                             }).ToListAsync();

        response.Data = folders;
        return response;
    }

    public async Task<ServiceResult<GetFolderViewCardInformationResponse>> GetFilePathAsync(List<int> filesIds, int currentFolderId)
    {
        var response = new ServiceResult<GetFolderViewCardInformationResponse>();

        var filePathName = await _context.Folders.Where(x => filesIds.Contains(x.ID) || x.ID == currentFolderId)
            .OrderBy(x => x.ID)
            .Select(x => x.Name)
            .ToListAsync();

        var imagesCount = await _context.FolderImages.CountAsync(x => x.FolderID == currentFolderId);

        var data = new GetFolderViewCardInformationResponse
        {
            FilesCount = imagesCount,
            Paths = filePathName
        };


        response.Data = data;
        return response;
    }

    public async Task<List<GetAllFoldersResponse>> GetTransferFolders()
    {
        var response = new List<GetAllFoldersResponse>();

        var foldersQuery = await (from folder in _context.Folders
            select new GetAllFoldersResponse
            {
                Id = folder.ID,
                Name = folder.Name
            }).ToListAsync();

        response = foldersQuery;
        return response;
    }

    public async Task<ServiceResult> TransferPhoto(TransferPhotoRequest request)
    {
        var response = new ServiceResult();

        var image = await _context.FolderImages.FirstOrDefaultAsync(x => x.ID == request.ImageID);
        if (image == null)
        {
            response.SetError(_localizer["RecordNotFound"]);
            return response;
        }
     
        image.FolderID = request.FolderID;
        
        _context.FolderImages.Update(image);
        await _context.SaveChangesAsync();

        return response;
    }


    #region Delete Folder Helper Method

    private async Task<List<Folder>> GetFolderWithChildFolders(Folder folder)
    {
        var folders = new List<Folder>();
        folders.Add(folder);

        var childFolders = await _context.Folders.Where(x => x.ParentID == folder.ID).ToListAsync();
        foreach (var childFolder in childFolders)
        {
            folders.AddRange(await GetFolderWithChildFolders(childFolder));
        }
        return folders;
    }



    #endregion
    public async Task<ServiceResult> ChangeImageNameAsync(ChangeImageNameRequest request)
    {
        var result = new ServiceResult();
        var folderImage = await _context.FolderImages.FirstOrDefaultAsync(x => x.ID == request.ID);
        if (folderImage != null)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(request.ImageName);
            var fileExtension = Path.GetExtension(folderImage.ImageName);

            folderImage.ImageName = fileNameWithoutExtension.TrimEnd() + fileExtension;

            _context.FolderImages.Update(folderImage);
            await _context.SaveChangesAsync();
        }

        return result;
    }

}