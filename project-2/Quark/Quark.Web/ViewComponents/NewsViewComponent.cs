using Microsoft.AspNetCore.Mvc;
using Quark.Business.Caching.Interfaces;
using Quark.Web.ViewComponents.Models;

namespace Quark.Web.ViewComponents
{
    public class NewsViewComponent : ViewComponent
    {
        private readonly INewsCachingService _newsCachingService;
        public NewsViewComponent(INewsCachingService newsCachingService)
        {
            _newsCachingService = newsCachingService;
        }

        public async Task<IViewComponentResult> InvokeAsync(NewsViewComponentModel model)
        {
			var newsList = await _newsCachingService.GetMostViewNews();
			newsList.Data = newsList.Data.Take(model.NewsCount).ToList();

			switch (model.MethodName)
            {
                case "MostViewedNews":
                    return View($"~/Views/Shared/Components/News/{model.MethodName}.cshtml", newsList.Data);                
                case "SurveyListMostViewedNews":
                    return View($"~/Views/Shared/Components/News/{model.MethodName}.cshtml", newsList.Data);                
                case "CategoryListMostViewedNews":
                    return View($"~/Views/Shared/Components/News/{model.MethodName}.cshtml", newsList.Data);                
                case "ZodiacSignListMostViewedNews":
                    return View($"~/Views/Shared/Components/News/{model.MethodName}.cshtml", newsList.Data);
                default:
                    return View();
            }
        }
    }
}
