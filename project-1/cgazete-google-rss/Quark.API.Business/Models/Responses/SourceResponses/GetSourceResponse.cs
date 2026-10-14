using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark.API.Business.Models.Responses.SourceResponses
{
    public class GetSourceResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
