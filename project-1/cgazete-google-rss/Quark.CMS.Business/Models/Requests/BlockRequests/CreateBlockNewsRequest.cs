namespace Quark.CMS.Business.Models.Requests.BlockRequests;

public class CreateBlockNewsRequest
{
    public int BlockID { get; set; }
    public List<SelectedNewsItems> SelectedNewsItems { get; set; }
}
