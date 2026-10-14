using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.Authors")]
public class Author
{
    public int ID { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Photo { get; set; }
    public bool IsEnabled { get; set; }
    public int Order { get; set; }
    public bool ShowInHome { get; set; }
    public bool ShowDailyColumnArticles { get; set; }
    public string ClichePhoto { get; set; }
    public string Url { get; set; }
    public string SeoTitle { get; set; }
    public string SeoDescription { get; set; }
    public string About { get; set; }
    public string ColumnName { get; set; }
    public string Breadcrumb { get; set; }
    public string FacebookUrl { get; set; }
    public string XUrl { get; set; }
    public string YoutubeUrl { get; set; }
    public string InstagramUrl { get; set; }
    public string LinkedinUrl { get; set; }
    public string LastNewsTitle { get; set; }
    public int? TotalNewsCount { get; set; } = 0;


    [NotMapped]
    public string FullName => $"{Firstname} {Lastname}";
}