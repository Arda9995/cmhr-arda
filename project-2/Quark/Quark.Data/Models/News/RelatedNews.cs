using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models
{
    [Table("News.NewsRelated")]

    public class RelatedNews
    {
        public int ID { get; set; }
        public long NewsID { get; set; }
        public long RelatedNewsID { get; set; }
        public int Order { get; set; }
    }
}