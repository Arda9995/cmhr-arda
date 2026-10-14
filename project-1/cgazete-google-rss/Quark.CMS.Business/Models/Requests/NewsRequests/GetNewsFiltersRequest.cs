using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Caching.Models.Responses.NewsTypeCachingResponses;

namespace Quark.CMS.Business.Models.Requests.NewsRequests;

public class GetNewsFiltersRequest
{
    public string SearchValue { get; set; }
    public List<SelectListItem> Authors { get; set; }
    public int SelectedAuthorID { get; set; }
    public string CreatedDate { get; set; }
    public string PublishDate { get; set; }
    public List<SelectListItem> StatusList { get; set; }
    public int SelectedStatusID { get; set; }
    public string Tags { get; set; }
    public List<SelectListItem> Categories { get; set; }
    public int SelectedCategoryID { get; set; }
    public List<SelectListItem> Types { get; set; }
    public List<SelectListItem> Users { get; set; }
    public int SelectedUserID { get; set; }
    public int SelectedTypeID { get; set; }
    public List<NewsTypeCachingResponse> CreateTypes { get; set; }
    public int SelectedViewRange { get; set; }
    public List<SelectListItem> Sources { get; set; }
    public int SelectedSourceID { get; set; }
    public bool HasCreateUpdateRight { get; set; }
}

public class TagItem
{
    public string Value { get; set; }
}