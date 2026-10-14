using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark.CMS.Business.Models.Responses.HomeResponses
{
    public class GetNewsDetailsForWidgetResponse
    {
        public long NewsID { get; set; }
        public string NewsTitle { get; set; }
        public int TagCount { get; set; }
        public string NewsSourceName { get; set; }
        public string NewsSourceIDs { get; set; }
        public string NewsTypeName { get; set; }
        public string CategoryName { get; set; }
        public int? Impression { get; set; }
        public string NewsImageURL { get; set; }
    }
}
