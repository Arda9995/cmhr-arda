using Quark.Business.Caching.Models.Responses.MenuCachingResponses;

namespace Quark.API.Business.Models.Responses.MenuResponses;

public class ListMenusResponse
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool OpenInNewWindow { get; set; }
    public int Order { get; set; }
    public string URL { get; set; }
    public int? ParentID { get; set; }
    public List<MenuCachingResponse> Children { get; set; }
}