using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.SystemLogRequests;

public class ListFiltersRequest
{      
    public List<SelectListItem> Types { get; set; }
    public int SelectedTypeID { get; set; }
}