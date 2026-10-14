using AvvaMobile.Core.ExcelExport;

namespace Quark.CMS.Business.Models.Responses.NewsPhotoTypeResponses
{
    public class ListResponseExcelItem
    {
        public int ID { get; set; }
        public string Key { get; set; }
        [ExcelColumnName("Adı")]
        public string Name { get; set; }
        [ExcelColumnName("Genişlik")]
        public int Width { get; set; }
        [ExcelColumnName("Uzunluk")]
        public int Height { get; set; }
        [ExcelColumnName("Durumu")]
        public string IsEnabledText { get; set; }
        [ExcelColumnName("Zorunlu")]
        public string IsRequiredText { get; set; }
        [ExcelColumnName("Sıra")]
        public int Order { get; set; }
    }
}
