using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.BlockRequests;

public class ListFiltersRequest
{
    public bool IsEnabled { get; set; }
    public List<SelectListItem> Pages { get; set; }
    public int SelectedPageID { get; set; }
}