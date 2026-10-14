using AvvaMobile.Core;
using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CropperRequests;
using Quark.CMS.Business.Models.Requests.NewsRequests;
using Quark.Data.Models;
using Quark.Resources;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.NEWS)]
public class NewsController(
    INewsService _service,
    IStringLocalizer<CommonResource> localizer,
    ICategoryService _categoryService,
    IWebHostEnvironment _env)
    : BaseController
{
    public async Task<IActionResult> List()
    {
        var result = await _service.GetAllPagedFilters();
        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPaged(GetNewsFiltersRequest request)
    {
        var result = await _service.GetAllPaged(new BaseDataTableRequest(Request), request);
        Message(result);
        return Json(result);
    }

    private void LogError<T>(string Title, T request)
    {
        try
        {
            var jsonRequest = JsonConvert.SerializeObject(request);
            string message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Title : {Title}\nRequest: {jsonRequest}\n\n";

            var logFilePath = Path.Combine(_env.WebRootPath, "errors.txt");
            System.IO.File.AppendAllText(logFilePath, message);
        }
        catch (Exception fileEx)
        {
            Console.WriteLine("LogErrorToFile hatas�: " + fileEx.Message);
        }
    }

    public async Task<IActionResult> Create(int id, int agencyNewsID = 0)
    {
        LogError("CONTROLLER : Create News With ID", id);
        switch (id)
        {
            case (int)NewsTypesEnum.News:
                var newsResponse = await _service.Create(id, agencyNewsID);
                return View("NewsCreate", newsResponse.Data);
            case (int)NewsTypesEnum.PhotoGallery:
                var photoGalleryResponse = await _service.Create(id);
                return View("PhotoGalleryCreate", photoGalleryResponse.Data);
            case (int)NewsTypesEnum.VideoGallery:
                var videoGalleryResponse = await _service.Create(id);
                return View("VideoGalleryCreate", videoGalleryResponse.Data);
            case (int)NewsTypesEnum.ColumnArticles:
                var columnArticleResponse = await _service.Create(id);
                return View("ColumnArticlesCreate", columnArticleResponse.Data);
            case (int)NewsTypesEnum.GovernmentAnnouncement:
                var governmentAnnouncementResponse = await _service.Create(id);
                return View("GovernmentAnnouncementCreate", governmentAnnouncementResponse.Data);
            case (int)NewsTypesEnum.Ad:
                var urlRedirectionResponse = await _service.Create(id);
                return View("URLRedirectionCreate", urlRedirectionResponse.Data);
            case (int)NewsTypesEnum.CaricaturistNews:
                var caricaturistNewsResponse = await _service.Create(id);
                return View("CaricaturistNewsCreate", caricaturistNewsResponse.Data);
            case (int)NewsTypesEnum.ZodiacSigns:
                var zodiacSignsResponse = await _service.Create(id);
                return View("ZodiacSignCreate", zodiacSignsResponse.Data);
            case (int)NewsTypesEnum.Survey:
                var SurveyResponse = await _service.Create(id);
                return View("SurveyNewsCreate", SurveyResponse.Data);
            default:
                var errorResult = new ServiceResult();
                errorResult.SetError(localizer["RecordNotFound"]);
                Message(errorResult);
                return RedirectToAction("List");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewsRequest request)
    {
        LogError("CONTROLLER : Create News With Request", request);
        var response = await _service.Create(request);

        if (!response.IsSuccess)
        {
            Message(response);
            switch (request.NewsTypeID)
            {
                case (int)NewsTypesEnum.News:
                    return View("NewsCreate", response.Data);
                case (int)NewsTypesEnum.PhotoGallery:
                    return View("PhotoGalleryCreate", response.Data);
                case (int)NewsTypesEnum.VideoGallery:
                    return View("VideoGalleryCreate", response.Data);
                case (int)NewsTypesEnum.ColumnArticles:
                    return View("ColumnArticlesCreate", response.Data);
                case (int)NewsTypesEnum.GovernmentAnnouncement:
                    return View("GovernmentAnnouncementCreate", response.Data);
                case (int)NewsTypesEnum.Ad:
                    return View("URLRedirectionCreate", response.Data);
                case (int)NewsTypesEnum.CaricaturistNews:
                    return View("CaricaturistNewsCreate", response.Data);
                case (int)NewsTypesEnum.ZodiacSigns:
                    return View("ZodiacSignCreate", response.Data);
                case (int)NewsTypesEnum.Survey:
                    return View("SurveyNewsCreate", response.Data);
                default:
                    var errorResult = new ServiceResult();
                    errorResult.SetError(localizer["RecordNotFound"]);
                    Message(errorResult);
                    return RedirectToAction("List");
            }
        }

        return Redirect($"/News/Details/{request.ID}");
    }


    public async Task<IActionResult> Update(long id)
    {
        var newsTypeResponse = await _service.GetNewsTypeID(id);
        if (!newsTypeResponse.IsSuccess)
        {
            Message(newsTypeResponse);
            return RedirectToAction(nameof(List));
        }

        switch ((NewsTypesEnum)newsTypeResponse.Data)
        {
            case NewsTypesEnum.News:
                var newsResponse = await _service.Update(id);
                if (!newsResponse.IsSuccess)
                    return RedirectToAction("List");
                return View("NewsUpdate", newsResponse.Data);

            case NewsTypesEnum.PhotoGallery:
                var photoGalleryResponse = await _service.Update(id);
                if (!photoGalleryResponse.IsSuccess)
                    return RedirectToAction("List");
                return View("PhotoGalleryUpdate", photoGalleryResponse.Data);

            case NewsTypesEnum.ColumnArticles:
                var columnArticleResponse = await _service.Update(id);
                if (!columnArticleResponse.IsSuccess)
                    return RedirectToAction("List");
                return View("ColumnArticlesUpdate", columnArticleResponse.Data);

            case NewsTypesEnum.VideoGallery:
                var videoGalleryResponse = await _service.Update(id);
                if (!videoGalleryResponse.IsSuccess)
                    return RedirectToAction("List");
                return View("VideoGalleryUpdate", videoGalleryResponse.Data);

            case NewsTypesEnum.GovernmentAnnouncement:
                var governmentAnnouncementResponse = await _service.Update(id);
                if (!governmentAnnouncementResponse.IsSuccess)
                    return RedirectToAction("List");
                return View("GovernmentAnnouncementUpdate", governmentAnnouncementResponse.Data);

            case NewsTypesEnum.Ad:
                var urlRedirectionResponse = await _service.Update(id);
                if (!urlRedirectionResponse.IsSuccess)
                    return RedirectToAction("List");
                return View("URLRedirectionUpdate", urlRedirectionResponse.Data);

            case NewsTypesEnum.CaricaturistNews:
                var caricaturistNewsResponse = await _service.Update(id);
                if (!caricaturistNewsResponse.IsSuccess)
                    return RedirectToAction("List");
                return View("CaricaturistNewsUpdate", caricaturistNewsResponse.Data);

            case NewsTypesEnum.ZodiacSigns:
                var zodiacSignsResponse = await _service.Update(id);
                if (!zodiacSignsResponse.IsSuccess)
                    return RedirectToAction("List");
                return View("ZodiacSignUpdate", zodiacSignsResponse.Data);

            case NewsTypesEnum.Survey:
                var surveyResponse = await _service.Update(id);
                if (!surveyResponse.IsSuccess)
                    return RedirectToAction("List");
                return View("SurveyNewsUpdate", surveyResponse.Data);

            default:
                return RedirectToAction("List");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update(NewsRequest request)
    {
        var response = await _service.Update(request);
        if (!response.IsSuccess)
        {
            Message(response);
            switch (request.NewsTypeID)
            {
                case (int)NewsTypesEnum.News:
                    var newsResponse = await _service.Update(request);
                    return View("NewsUpdate", newsResponse.Data);
                case (int)NewsTypesEnum.PhotoGallery:
                    var photoGalleryResponse = await _service.Update(request);
                    return View("PhotoGalleryUpdate", photoGalleryResponse.Data);
                case (int)NewsTypesEnum.ColumnArticles:
                    var columnArticleResponse = await _service.Update(request);
                    return View("ColumnArticlesUpdate", columnArticleResponse.Data);
                case (int)NewsTypesEnum.VideoGallery:
                    var videoGalleryResponse = await _service.Update(request);
                    return View("VideoGalleryUpdate", videoGalleryResponse.Data);
                case (int)NewsTypesEnum.GovernmentAnnouncement:
                    var governmentAnnouncementResponse = await _service.Update(request);
                    return View("GovernmentAnnouncementUpdate", governmentAnnouncementResponse.Data);
                case (int)NewsTypesEnum.Ad:
                    var urlRedirectionResponse = await _service.Update(request);
                    return View("URLRedirectionUpdate", urlRedirectionResponse.Data);
                case (int)NewsTypesEnum.CaricaturistNews:
                    var caricaturistNewsResponse = await _service.Update(request);
                    return View("CaricaturistNewsUpdate", caricaturistNewsResponse.Data);
                case (int)NewsTypesEnum.ZodiacSigns:
                    var zodiacSignsResponse = await _service.Update(request);
                    return View("ZodiacSignUpdate", zodiacSignsResponse.Data);
                case (int)NewsTypesEnum.Survey:
                    var surveyResponse = await _service.Update(request);
                    return View("SurveyNewsUpdate", surveyResponse.Data);
                default:
                    var errorResult = new ServiceResult();
                    errorResult.SetError(localizer["RecordNotFound"]);
                    Message(errorResult);
                    return RedirectToAction("List");
            }
        }

        return Redirect($"/News/Details/{request.ID}");
    }

    public async Task<IActionResult> UploadImage(IFormFile file, int photoTypeId)
    {
        var response = await _service.UploadImage(file, photoTypeId);
        return Json(response);
    }

    public async Task<IActionResult> Details(long id)
    {
        var result = await _service.Details(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return Redirect("/News/List");
        }

        return View(result.Data);
    }

    public async Task<IActionResult> UpdateSocialMedias(long id)
    {
        var result = await _service.UpdateSocialMedias(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction(nameof(List));
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSocialMedias(SocialMediaRequest request)
    {
        var result = await _service.UpdateSocialMedias(request);
        Message(result);
        if (!result.IsSuccess)
        {
            return View(result.Data);
        }

        return Redirect($"/News/Details/{request.ID}");
    }

    [HttpPost]
    public async Task<IActionResult> PublishNowOrScheduled(long id)
    {
        var result = await _service.PublishNowOrScheduled(id);

        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result });
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UnPublish(long id, string description)
    {
        var result = await _service.UnPublish(id, description);

        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result });
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UnSchedule(long id)
    {
        var result = await _service.UnSchedule(id);

        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result });
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Lock(long id)
    {
        var result = await _service.Lock(id);

        if (!result.IsSuccess)
        {
            Message(result);
            return Json(new { success = false, message = result });
        }

        return Json(new { success = true, message = result });
    }

    [HttpPost]
    public async Task<IActionResult> UnLock(long id)
    {
        var result = await _service.UnLock(id);

        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result });
        }

        return Json(new { success = true });
    }

    public async Task<IActionResult> Version(long id)
    {
        var result = await _service.Version(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction("List");
        }

        return View(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeVersion(long id, int newsId)
    {
        var result = await _service.ChangeVersion(id);
        Message(result);
        return Ok(result);
    }

    public async Task<IActionResult> Live(long id)
    {
        var result = await _service.Live(id);
        if (!result.IsSuccess)
        {
            Message(result);
            return RedirectToAction("List");
        }

        return View(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetEditTemplates()
    {
        var response = await _service.GetTemplateList();
        return Json(response.Data);
    }

    [HttpPost]
    public async Task<IActionResult> PreparePhoto(PrepareEditModalRequest request)
    {
        var response = await _service.PreparePhotoAsync(request);
        return Json(response);
    }

    public IActionResult PhotoEdit([FromQuery] EditRequest request)
    {
        return View(request);
    }

    [HttpPost]
    public async Task<IActionResult> SaveEditedPhoto(SaveEditedPhotoRequest request)
    {
        var result = await _service.SaveEditedPhotoAsync(request);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> GetRelatedNewsPaged(long id)
    {
        var result = await _service.GetRelatedNewsPaged(new BaseDataTableRequest(Request), id);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> GetAllPublishedNewsPaged(long id)
    {
        var result = await _service.GetAllPublishedNewsPaged(new BaseDataTableRequest(Request), id);
        return Json(result);
    }

    public async Task<IActionResult> RemoveRelatedNews(long id)
    {
        var result = await _service.RemoveRelatedNews(id);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRelatedNewsByNewsID(long newsID, long relatedNewsID)
    {
        var result = await _service.RemoveRelatedNewsByNewsID(newsID, relatedNewsID);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> SelectRelatedNews(SelectRelatedNewsRequest request)
    {
        var result = await _service.SelectRelatedNews(request);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> ChangePhotoGalleryItemOrder(long id, string[] orders)
    {
        var result = await _service.ChangePhotoGalleryItemOrder(id, orders);
        return Json(result);
    }

    public async Task<IActionResult> GetDetailsImpressions(long id)
    {
        var result = await _service.GetDetailsImpressions(id);
        return Json(result);
    }

    [HttpGet]
    public async Task<JsonResult> LoadTree(int? parent, bool onlyEnableds)
    {
        var result = await _categoryService.GetCategoriesTree(parent, onlyEnableds);
        return Json(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePhotoGalleryItem(long newsId, string subtitle, string content,
        IFormFile file)
    {
        var result = await _service.CreatePhotoGalleryItem(newsId, subtitle, content, file);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePhotoGalleryItem(long itemId, string subtitle, string content,
        IFormFile file)
    {
        var result = await _service.UpdatePhotoGalleryItem(itemId, subtitle, content, file);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> DeletePhotoGalleryItem(long id)
    {
        var result = await _service.DeletePhotoGalleryItem(id);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> SavePhotoGalleryItems([FromBody] PhotoGalleryItemsSaveRequest request)
    {
        var result = await _service.SavePhotoGalleryItems(request);
        return Json(result);
    }

    [HttpPost]
    public async Task<IActionResult> ClearCloudflareCache(string url)
    {
        var result = await _service.ClearCloudflareCache(url);
        return Ok(url);
    }
}