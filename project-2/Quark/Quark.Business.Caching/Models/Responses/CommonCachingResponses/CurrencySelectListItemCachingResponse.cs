using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.Business.Caching.Models.Responses.CommonCachingResponses;

public class CurrencySelectListItemCachingResponse
{
    public List<SelectListItem> CurrenctSelectListItem { get; set; }
}