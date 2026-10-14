
using AvvaMobile.Core.ExcelExport;

namespace Quark.CMS.Business.Models.Responses.TagResponses
{
	public class ListResponseItem
	{
		public long ID { get; set; }
        [ExcelColumnName("Adı")]
        public string Name { get; set; }
		public string URL { get; set; }
		public string PhotoUrl { get; set; }
	}
}
