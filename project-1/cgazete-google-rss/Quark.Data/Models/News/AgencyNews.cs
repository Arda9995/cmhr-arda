using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.News
{
    [Table("News.AgencyNews")]

    public class AgencyNews
    {
        public int ID { get; set; }
        public int AgencyID { get; set; }
        public int CategoryID { get; set; }
        public string AgencyNewsID { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
    }
}