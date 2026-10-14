using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Member.Notifications")]
public class Notification
{
    public int ID { get; set; }
    public int? NotificationSenderID { get; set; }
    public int MemberID { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string ImageUrl { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedDate { get; set; }
}