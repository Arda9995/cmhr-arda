using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Quark.Data.Models.XML
{
    [Serializable]
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class Sitemap
    {
        public Sitemap()
        {
            Urls = new List<SitemapUrl>();
        }

        [XmlElement("url")]
        public List<SitemapUrl> Urls { get; set; }
    }
} 