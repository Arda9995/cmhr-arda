using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark.CMS.Business.Models.Responses.HomeResponses
{
    public class GetNewsChartDataByCategoriesResponse
    {
        public List<string> Categories{ get; set; }
        public List<int> NewsCountsByCategory{ get; set; }
    }
}
