using System;
using System.Xml.Serialization;

namespace Quark.Data.Models.XML
{
    [Serializable]
    public class SitemapUrl
    {
        [XmlElement("loc")]
        public string Location { get; set; }

        [XmlElement("lastmod")]
        public string LastModified { get; set; }

        [XmlElement("changefreq")]
        public string ChangeFrequency { get; set; }

        [XmlElement("priority")]
        public string Priority { get; set; }

        [XmlIgnore]
        public long NewsId { get; set; }
    }
} 