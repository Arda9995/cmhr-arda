using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("News.Status")]
public class NewsStatus
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public bool IsSelectableItem { get; set; }
}

public enum NewsStatusEnum : int
{
    Draft = 1,
    Published = 2,
    Unpublished = 3,
    Scheduled = 4
}