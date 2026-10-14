using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.PhotoEditor;

public class EditPhotoRequest
{
    public int ID { get; set; }
    public string OriginalBase64Image { get; set; }
    public string EditedBase64Image { get; set; }
    public string Text { get; set; }
    public int TextFont { get; set; }
    public int TextColorId { get; set; }
    public List<SelectListItem> TextColors { get; set; }
    public int LocationId { get; set; }
    public List<SelectListItem> Locations { get; set; }
}