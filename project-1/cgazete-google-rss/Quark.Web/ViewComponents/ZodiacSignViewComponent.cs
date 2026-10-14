using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.PageResponses;

namespace Quark.Web.ViewComponents;

public class ZodiacSignViewComponent : ViewComponent
{
    
    private readonly INewsService _newsService;
    public ZodiacSignViewComponent(INewsService newsService)
    {
        _newsService = newsService;
    }
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var zodiacSignNews = await _newsService.GetZodiacSignNewsListForIndex();
        if (!zodiacSignNews.IsSuccess)
        {
            return View("");
        }
        else
        {
            var zodiacSignListItem = zodiacSignNews.Data.ZodiacSignItems
                .Select(x => new ZodiacSignBlock
                {
                    NewsID = x.NewsID,
                    Slug = x.Slug,
                    Icon = x.Icon,
                    PublishDate = x.PublishDate,
                    ZodiacSignType = x.ZodiacSignType
                })
                .ToList();
            
            return View("~/Views/Shared/Components/Widgets/ZodiacSignWidget.cshtml", zodiacSignListItem);
        }
    }
}