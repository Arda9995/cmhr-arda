using AvvaMobile.Core.Business;
using Quark.Data.Models;

namespace Quark.Business.Caching.Interfaces;

public interface INewsPhotosCachingService
{
    Task<ServiceResult> RefreshNewsPublishedPhotoGalleryItems(int newsId);
    Task<ServiceResult<List<NewsPublishedPhotoGalleryItem>>> GetNewsPublishedPhotoGalleryItems(int newsId);
}