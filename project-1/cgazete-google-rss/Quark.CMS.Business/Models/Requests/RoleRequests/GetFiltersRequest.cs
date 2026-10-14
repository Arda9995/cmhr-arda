using Quark.CMS.Business.Models.Requests.CommonRequests;

namespace Quark.CMS.Business.Models.Requests.RoleRequests
{
    public class GetFiltersRequest : ExcelExportRequest
    {       
        public bool IsEnabled { get; set; }
    }
}