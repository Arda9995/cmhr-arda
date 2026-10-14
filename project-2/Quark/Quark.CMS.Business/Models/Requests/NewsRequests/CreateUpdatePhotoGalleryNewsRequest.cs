using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.NewsRequests;

public class CreateUpdatePhotoGalleryNewsRequest
{
    public long ID { get; set; }
    public int NewsTypeID { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Slug { get; set; }
    public PhotoRequest Photo { get; set; }
    public int StatusID { get; set; }
    public List<SelectListItem> StatusIDs { get; set; }
    public int CategoryID { get; set; }
    public List<int> SelectedSubCategoryIDs { get; set; }
    public List<SelectListItem> CategoryIDs { get; set; }
    public string Tags { get; set; }
    public string BaseURL { get; set; }
    public string ScheduleDate { get; set; }
    public bool IsLockedByCurrentUser { get; set; }
    public List<PromptResponseItem> TitlePrompts { get; set; }
    public List<PromptResponseItem> SubtitlePrompts { get; set; }
    public List<PromptResponseItem> ContentPrompts { get; set; }
    public List<PromptResponseItem> TagPrompts { get; set; }
    public List<PhotoGalleryItemRequest> PhotoGalleryItems { get; set; }
   
}


