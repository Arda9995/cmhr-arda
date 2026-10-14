using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Common
{
    [Table("Common.UrlMappings")]
    public class UrlMapping
    {
        public int ID { get; set; }
        public int EntityTypeID { get; set; }
        public string OldSlug { get; set; }
        public string NewSlug { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
