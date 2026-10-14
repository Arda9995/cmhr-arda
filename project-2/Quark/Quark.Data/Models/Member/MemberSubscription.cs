using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models;

[Table("Member.Subscriptions")]
public class MemberSubscription
{
    public int ID { get; set; }
    public int MemberID { get; set; }
    public string SubscriptionReferenceCode { get; set; }
    public string SubscriptionParentReferenceCode { get; set; }
    public string ConversationId { get; set; }
    public string SubscriptionStatus { get; set; }
    public DateTime SubscriptionStartDate { get; set; } = DateTime.Now;
    public DateTime? SubscriptionEndDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public int PlanID { get; set; }
}