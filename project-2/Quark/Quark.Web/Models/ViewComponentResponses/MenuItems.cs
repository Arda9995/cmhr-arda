using Quark.Business.Caching.Models.Responses.MenuCachingResponses;

namespace Quark.Web.Models.ViewComponentResponses
{
    public class MenuItems
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public int? ParentID { get; set; }
        public string URL { get; set; }
        public bool OpenInNewWindow { get; set; }
        public int Order { get; set; }
        public List<MenuCachingResponse> Children { get; set; }
    }
}
