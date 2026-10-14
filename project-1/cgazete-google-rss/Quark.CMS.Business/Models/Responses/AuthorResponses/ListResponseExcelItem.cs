using AvvaMobile.Core.ExcelExport;

namespace Quark.CMS.Business.Models.Responses.AuthorResponses
{
    public class ListResponseExcelItem
    {
        public int ID { get; set; }
        [ExcelColumnName("Adı")]
        public string Name { get; set; }
        [ExcelColumnName("Email")]
        public string Email { get; set; }
        [ExcelColumnName("Durumu")]
        public string IsEnabledText { get; set; }
    }
}
