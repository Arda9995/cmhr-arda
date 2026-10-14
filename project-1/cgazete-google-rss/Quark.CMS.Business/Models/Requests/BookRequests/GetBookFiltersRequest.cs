using Quark.CMS.Business.Models.Requests.CommonRequests;

namespace Quark.CMS.Business.Models.Requests.BookRequests
{
    public class GetBookFiltersRequest : ExcelExportRequest
    {
        public bool IsEnabled { get; set; } = true;
    }
}