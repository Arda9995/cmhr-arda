using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.NewsPhotoTypes")]
public class NewsPhotoType
{
    public int ID { get; set; }
    public string Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
}

public enum NewsPhotoTypesEnum
{
    MainPhoto = 1,
    CoverPhoto = 2,
    PhotoGalleryItemPhoto = 9,
    GoogleAiHighResPhoto = 10
}