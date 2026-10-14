using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Responses.BlokResponses;

public class SelectListBlockItemResponse
{
    public List<SelectListItem> BlockTypes { get; set; }
    public List<SelectListItem> Users { get; set; }
}