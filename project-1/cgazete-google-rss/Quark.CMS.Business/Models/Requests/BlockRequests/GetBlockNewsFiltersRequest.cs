using Quark.CMS.Business.Models.Requests.CommonRequests;

namespace Quark.CMS.Business.Models.Requests.BlockRequests;
public class GetBlockNewsFiltersRequest : PagingRequest
{
    public string SearchValue { get; set; }
    public List<SelectedNewsItems> SelectedNewsItems { get; set; }
}

public class SelectedNewsItems
{
    public long NewsID { get; set; }
    public int Order { get; set; }
}