using AvvaMobile.Core.APIControllers;
using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Requests.NewsRequests;

namespace Quark.API.Controllers;

[ApiController]
public class NewsController(INewsService newsService, IPageService pageService) : ApiPublicBaseController
{
    private readonly IPageService _pageService = pageService;
    private readonly string mainPageKey = "AnaSayfa";

    [HttpGet]
    public IActionResult Index()
    {
        return Ok();
    }

    //[HttpGet]
    //public async Task<IActionResult> RemoveNews()
    //{
    //          long[] idList = new long[] { 2400003};

    //          var result = await _newsService.RemoveNewsByIDList(idList);
    //	return Ok();
    //}

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var newsType = await newsService.GetNewsType(id);
        if (!newsType.IsSuccess)
            return BadRequest(newsType.Message);

        var newsDetail = await newsService.GetNewsDetail(id);
        if (!newsDetail.IsSuccess)
            return BadRequest(newsDetail.Message);

        var pageResponse = new
        {
            NewsType = newsType.Data,
            NewsDetail = newsDetail.Data
        };

        var response = new ServiceResult<object>()
        {
            IsSuccess = true,
            Data = pageResponse
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetMostSearchedNewsPage()
    {
        var pageNews = await newsService.GetMostSearchedNewsBlock();
        if (!pageNews.IsSuccess)
            return BadRequest(pageNews.Message);

        return Ok(pageNews);
    }

    [HttpGet]
    public async Task<IActionResult> SearchedNewsPage([FromQuery] string search)
    {
        var newsList = await newsService.GetNewsListBySearchedKey(search);
        if (!newsList.IsSuccess)
            return BadRequest(newsList.Message);

        return Ok(newsList);
    }

    [HttpPost]
    public async Task<IActionResult> NewsSearch(LiveSearchRequest request)
    {
        var newsList = await newsService.GetNewsListBySearchKey(request.Search);
        if (!newsList.IsSuccess)
            return BadRequest(newsList.Message);

        return Ok(newsList);
    }

    [HttpPost]
    public async Task<IActionResult> VoteSurvey(int surveyId, int optionId)
    {
        var newsList = await newsService.VoteSurvey(surveyId, optionId);
        return Ok(newsList);
    }

    [HttpGet]
    public async Task<IActionResult> CreateElasticDetail(int id)
    {
        var newsDetail = await newsService.CreateNewsDetail(id);
        return Ok(newsDetail);
    }

    [HttpGet]
    public async Task<IActionResult> SearchPageLoadMore([FromQuery] string search, [FromQuery] int page,
        [FromQuery] string dateRange)
    {
        var newsResult = await newsService.GetMoreNewsBySearchKey(search, page, dateRange);
        if (!newsResult.IsSuccess)
            return BadRequest(newsResult.Message);

        var pageResponse = new
        {
            Items = newsResult.Data.Items,
            HasMoreItems = newsResult.Data.HasMoreItems,
            TotalCount = newsResult.Data.TotalCount
        };

        var response = new ServiceResult<object>()
        {
            IsSuccess = true,
            Data = pageResponse
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> TestMetod()
    {
        var pageNews = await newsService.TestMethod();
        return Ok(pageNews);
    }
}