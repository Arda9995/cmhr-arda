namespace Quark.API.Business.Models.Requests.CommonRequest;

public class CreateWebSetrowSubscriptionRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
    public List<int> GroupIDs { get; set; }
}