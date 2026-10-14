using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark.CMS.Business.Models.Responses.HomeResponses
{
    public class GetLastPublishedNewsResponse
    {
        public long NewsID { get; set; }
        public int? AuthorID{ get; set; }
        public string NewsImageURL { get; set; }
        public string AuthorImageURL { get; set; }
        public string AuthorName { get; set; }
        public string NewsTitle { get; set; }
        public string NewsSubtitle { get; set; }
        public string PublishDate { get; set; }
    }
}
