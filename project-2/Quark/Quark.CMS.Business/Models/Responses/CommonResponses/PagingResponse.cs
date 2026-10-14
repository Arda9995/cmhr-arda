using AvvaMobile.Core.Business;

namespace Quark.CMS.Business.Models.Responses.CommonResponses;

public class PagingResponse<TData> : ServiceResult<IEnumerable<TData>>
{
    public int PageCount { get; set; }
    public int CurrentPage { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    public string SearchValue { get; set; }
}