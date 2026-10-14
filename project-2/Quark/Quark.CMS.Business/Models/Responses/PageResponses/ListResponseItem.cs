using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark.CMS.Business.Models.Responses.PageResponses
{
    public class ListResponseItem
    {
        public int ID { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}