namespace Quark.CMS.Business.Models.Responses.CommonResponses
{
    public class ExcelExportResponse
    {
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}
