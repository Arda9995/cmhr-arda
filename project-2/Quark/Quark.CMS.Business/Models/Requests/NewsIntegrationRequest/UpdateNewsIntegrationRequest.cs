using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.NewsIntegrationRequest;

public class UpdateNewsIntegrationRequest
{
    public long ID { get; set; }
    public List<SelectListItem> Categories { get; set; }
    public int CategoryID { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}