using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.PromptRequests;

public class GetPromptFiltersRequest
{
    public List<SelectListItem> PromptTypes { get; set; }
    public int SelectedPromptTypeID { get; set; }
}