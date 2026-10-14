using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.MemberRequests
{
    public class ListFiltersRequest 
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
    }
}