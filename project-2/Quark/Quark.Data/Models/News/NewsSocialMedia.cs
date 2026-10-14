using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.SocialMedias")]
public class NewsSocialMedia
{
    public long NewsID { get; set; }
    public string FacebookTitle { get; set; }
    public string FacebookContent { get; set; }
    public string FacebookPhotoUrl { get; set; }
    public string XTitle { get; set; }
    public string XContent { get; set; }
    public string XPhotoUrl { get; set; }
    public string InstagramTitle { get; set; }
    public string InstagramContent { get; set; }
    public string InstagramPhotoUrl { get; set; }
}