using System;
using System.Xml.Serialization;

namespace Quark.Data.Models.XML
{
    [Serializable]
    public class RssItem
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("pubDate")]
        public string PubDate { get; set; }

        [XmlElement("guid")]
        public string Guid { get; set; }

        [XmlElement("category")]
        public string Category { get; set; }

        [XmlIgnore]
        public long NewsId { get; set; }
    }
} 