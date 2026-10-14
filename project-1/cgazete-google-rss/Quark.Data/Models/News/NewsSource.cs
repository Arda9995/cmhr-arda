using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.News
{
    [Table("News.NewsSources")]
	public class NewsSource
	{
        public int SourceID { get; set; }
        public int NewsID { get; set; }
    }
}
