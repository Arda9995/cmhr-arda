using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Models.Responses.NewsResponses;
using Quark.Business.Caching.Interfaces;

namespace Quark.Web.ViewComponents;

public class ColumArticlesWidgetComponent : ViewComponent
{
    private readonly INewsCachingService _newsCachingService;
    public ColumArticlesWidgetComponent(INewsCachingService newsCachingService)
    {
        _newsCachingService = newsCachingService;
    }
    
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var todayColumnsArticleCacheResponse = await _newsCachingService.GetAuthorSideColumnArticles();
        if (!todayColumnsArticleCacheResponse.IsSuccess)
        {
            return View();
        }
        else
        {
            var todayColumnsArticleCacheResponseData = todayColumnsArticleCacheResponse.Data
                .Select(x=> new TodaysColumnArticles()
            {
                NewsID = (int)x.NewsID,
                AuthorFullName = x.AuthorFullName,
                Slug = x.Slug,
                Headline = x.Headline,
                AuthorPhoto = x.AuthorPhoto,
                AuthorID = x.AuthorID,
                UpSubtitle = x.UpSubtitle,
            }).ToList();
            
            if (todayColumnsArticleCacheResponseData.Count > 0)
            {
                return View("~/Views/Shared/Components/Widgets/ColumnArticlesWidget.cshtml", todayColumnsArticleCacheResponseData);
            }
            else
            {
                return View();
            }
        }
    }
}