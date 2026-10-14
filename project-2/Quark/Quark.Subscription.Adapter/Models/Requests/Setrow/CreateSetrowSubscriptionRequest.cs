namespace Quark.Subscription.Adapter.Models.Requests.Setrow;

public class CreateSetrowSubscriptionRequest
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public int GroupID { get; set; }
}