using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Quark.Data.Models.XML
{
    [Serializable]
    [XmlRoot("rss")]
    public class RssFeed
    {
        public RssFeed()
        {
            Channel = new RssChannel();
            Version = "2.0";
        }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlElement("channel")]
        public RssChannel Channel { get; set; }
    }

    [Serializable]
    public class RssChannel
    {
        public RssChannel()
        {
            Items = new List<RssItem>();
        }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("language")]
        public string Language { get; set; }

        [XmlElement("copyright")]
        public string Copyright { get; set; }

        [XmlElement("lastBuildDate")]
        public string LastBuildDate { get; set; }

        [XmlElement("item")]
        public List<RssItem> Items { get; set; }
    }
} 