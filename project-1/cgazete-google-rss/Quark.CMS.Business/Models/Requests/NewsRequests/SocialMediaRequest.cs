using Microsoft.AspNetCore.Http;

namespace Quark.CMS.Business.Models.Requests.NewsRequests;

public class SocialMediaRequest
{
    public long ID { get; set; }
    public string FacebookTitle { get; set; }
    public string FacebookContent { get; set; }
    public string FacebookPhotoUrl { get; set; }
    public IFormFile FacebookPhoto { get; set; }
    public string XTitle { get; set; }
    public string XContent { get; set; }
    public string XPhotoUrl { get; set; }
    public IFormFile XPhoto { get; set; }

    public string InstagramTitle { get; set; }
    public string InstagramContent { get; set; }
    public string InstagramPhotoUrl { get; set; }
    public IFormFile InstagramPhoto { get; set; }

}