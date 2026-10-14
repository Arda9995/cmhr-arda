using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quark.CMS.Business.Models.Responses.BookResponses
{
    public class DetailsBooksNewResponseItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Authhor { get; set; }
        public bool IsEnabled { get; set; }
        public string BaseURL { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }

    }
}
