namespace Quark.Subscription.Adapter.Models.Requests.Setrow;

public class SetrowEmailRequest
{
    public string email { get; set; }
    public string name { get; set; }
    public string groupId { get; set; }
    public IysInfo iys { get; set; }
}

public class IysInfo
{
    public string emailAllowStatus { get; set; }
    public int emailOptInSource { get; set; }
    public string emailOptInDate { get; set; }
}