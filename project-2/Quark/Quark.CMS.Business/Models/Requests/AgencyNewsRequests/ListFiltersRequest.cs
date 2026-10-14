using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.AgencyNewsRequests
{
    public class ListFiltersRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsActive { get; set; }
        public List<SelectListItem> Agencies { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public int SelectedAgencyID { get; set; }
        public int  SelectedCategoryID { get; set; }
    }
}
