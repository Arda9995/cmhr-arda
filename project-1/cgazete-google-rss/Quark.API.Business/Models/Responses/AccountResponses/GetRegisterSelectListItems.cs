using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.API.Business.Models.Responses.AccountResponses
{
    public class GetRegisterSelectListItems
    {
        public List<SelectListItem> Cities { get; set; }
        public List<SelectListItem> Countries { get; set; }
    }
}
