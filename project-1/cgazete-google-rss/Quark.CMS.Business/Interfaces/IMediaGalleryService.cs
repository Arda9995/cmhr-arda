using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.MediaGallery;
using Quark.CMS.Business.Models.Responses.MediaGalleryResponses;

namespace Quark.CMS.Business.Interfaces;

public interface IMediaGalleryService
{
    Task<BaseDataTableResponse<List<GetAllFoldersResponse>>> GetAllFoldersAsync(BaseDataTableRequest tableRequest);
    Task<BaseDataTableResponse<List<FolderImageInfoResponse>>> GetFolderImagesAsync(BaseDataTableRequest tableRequest, GetImagesFilterRequest request);
    Task<BaseDataTableResponse<List<GetAllFoldersResponse>>> GetAllArchiveFoldersAsync(BaseDataTableRequest tableRequest);
    Task<BaseDataTableResponse<List<FolderImageInfoResponse>>> GetArchiveFolderImagesAsync(BaseDataTableRequest tableRequest, GetImagesFilterRequest request);
    Task<BaseDataTableResponse<List<FolderImageInfoResponse>>> GetFavouritesAsync(BaseDataTableRequest tableRequest, GetImagesFilterRequest request);
    Task<ServiceResult> AddFavourite(int imageId);
    Task<ServiceResult> DeleteFavourite(int imageId);
    Task<ServiceResult> AddFolderAsync(AddFolderRequest request);
    Task<ServiceResult<UpdateFolderRequest>> UpdateFolderAsync(int folderId);
    Task<ServiceResult> UpdateFolderAsync(UpdateFolderRequest request);
    Task<ServiceResult> UploadImageAsync(UploadImageRequest request);
    Task<ServiceResult> DeleteFolderAsync(int folderId);
    Task<ServiceResult> DeleteImageAsync(int imageId);
    Task<ServiceResult> ChangeImageNameAsync(ChangeImageNameRequest request);
    Task<ServiceResult<List<GetFolderTreeItemResponse>>> GetGalleryTreeAsync(int? parentId);
    Task<ServiceResult<List<GetFolderTreeItemResponse>>> GetArchiveTree(int? parentId);
    Task<ServiceResult<GetFolderViewCardInformationResponse>> GetFilePathAsync(List<int> filesIds,int currentFolderId);
    Task<List<GetAllFoldersResponse>> GetTransferFolders();
    Task<ServiceResult> TransferPhoto(TransferPhotoRequest request);
    
}