using Quark.Data.Models;

namespace Quark.Business.Caching.Models.Responses.NewsPhotosCachingResponse;

public class GetNewsPublishedPhotoGalleryItemResponse
{
    public int ID { get; set; }
    public List<NewsPublishedPhotoGalleryItem> NewsPublishedPhotoGalleryItems { get; set; }
}