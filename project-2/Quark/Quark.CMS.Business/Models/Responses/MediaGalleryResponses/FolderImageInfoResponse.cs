namespace Quark.CMS.Business.Models.Responses.MediaGalleryResponses;

public class FolderImageInfoResponse
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public string FileName { get; set; }
    public DateTime UploadDate { get; set; }
    public string FormattedUploadDate { get; set; }
    public string UploadedBy { get; set; }
    public bool IsFavourite { get; set; }
    public int FolderID { get; set; }
    public int ImageID { get; set; }
    public string Tooltip { get; set; }
}