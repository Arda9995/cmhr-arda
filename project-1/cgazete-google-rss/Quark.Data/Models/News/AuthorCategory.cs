using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.AuthorCategories")]
public class AuthorCategory
{
    public int AuthorID { get; set; }
    public int CategoryID { get; set; }
}