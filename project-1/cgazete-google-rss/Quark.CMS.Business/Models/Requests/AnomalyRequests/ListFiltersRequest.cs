using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.AnomalyRequests;

public class ListFiltersRequest
{
    public List<SelectListItem> DataTypes { get; set; }
    public int SelectedDataTypeID { get; set; }
    public List<SelectListItem> IssueTypes { get; set; }
    public int SelectedIssueTypeID { get; set; }
    public List<SelectListItem> Users { get; set; }
    public int SelectedUserID { get; set; }
    public bool OnlyShowUnResolved { get; set; }
}