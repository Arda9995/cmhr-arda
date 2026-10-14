using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.News
{
	[Table("News.NewsSecondaryCategories")]
	public class NewsSecondaryCategory
	{
        public int SecondaryCategoryID { get; set; }
        public int NewsID { get; set; }
    }
}
