using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Http;
using Quark.CMS.Business.Models.Requests.NewsRequests;
using Quark.CMS.Business.Models.Responses.NewsPhotoResponses;
using Quark.CMS.Business.Models.Responses.NewsResponses;

namespace Quark.CMS.Business.Interfaces
{
    public interface INewsService
    {
        Task<ServiceResult<NewsContentPhotoResponse>> UploadImage(IFormFile file, int photoTypeId);
        Task<ServiceResult<NewsRequest>> Create(int newsTypeID, int agenctNewsID = 0);
        Task<ServiceResult<NewsRequest>> Create(NewsRequest request);
        Task<ServiceResult<GetNewsFiltersRequest>> GetAllPagedFilters();
        Task<BaseDataTableResponse<List<ListResponseNewsItem>>> GetAllPaged(BaseDataTableRequest tableRequest, GetNewsFiltersRequest request);
        Task<ServiceResult<NewsRequest>> Update(long id);
        Task<ServiceResult<NewsRequest>> Update(NewsRequest request);
        Task<ServiceResult<DetailsNewsResponse>> Details(long id);
        Task<ServiceResult<SocialMediaRequest>> UpdateSocialMedias(long id);
        Task<ServiceResult<SocialMediaRequest>> UpdateSocialMedias(SocialMediaRequest request);
        Task<ServiceResult> PublishNowOrScheduled(long id);
        Task<ServiceResult> UnPublish(long id, string description);
        Task<ServiceResult> UnSchedule(long id);
        Task<ServiceResult> Lock(long id);
        Task<ServiceResult> UnLock(long id);
        Task<ServiceResult<NewsVersionLiveResponse>> Version(long id);
        Task<ServiceResult> ChangeVersion(long id);
        Task<ServiceResult<List<ListEditTemplateResponse>>> GetTemplateList();
        Task<ServiceResult<PrepareEditModalResponse>> PreparePhotoAsync(PrepareEditModalRequest request);
        Task<ServiceResult<SaveEditedPhotoResponse>> SaveEditedPhotoAsync(SaveEditedPhotoRequest request);
        Task<BaseDataTableResponse<List<ListResponseNewsItem>>> GetRelatedNewsPaged(BaseDataTableRequest tableRequest, long newsID);
        Task<BaseDataTableResponse<List<ListResponseNewsItem>>> GetAllPublishedNewsPaged(BaseDataTableRequest tableRequest, long newsID);
        Task<ServiceResult> RemoveRelatedNews(long id);
        Task<ServiceResult> RemoveRelatedNewsByNewsID(long newsID, long relatedNewsID);
        Task<ServiceResult> SelectRelatedNews(SelectRelatedNewsRequest request);
        Task<ServiceResult<int>> GetNewsTypeID(long id);
        Task<ServiceResult<NewsVersionLiveResponse>> Live(long id);
        Task<ServiceResult> ChangePhotoGalleryItemOrder(long id, string[] orders);
        Task<ServiceResult<DetailsImpressionsResponse>> GetDetailsImpressions(long id);
        Task<ServiceResult> CreatePhotoGalleryItem(long newsID, string subtitle, string content, IFormFile photo);
        Task<ServiceResult> UpdatePhotoGalleryItem(long itemID, string subtitle, string content, IFormFile photo);
        Task<ServiceResult> DeletePhotoGalleryItem(long itemID);
        Task<ServiceResult> SavePhotoGalleryItems(PhotoGalleryItemsSaveRequest request);
        Task<ServiceResult> ClearCloudflareCache(string url);
    }
}
