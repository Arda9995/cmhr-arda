using AvvaMobile.Core;
using AvvaMobile.Core.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Resources;

namespace Quark.CMS.Controllers;

public class CachesController: BaseController
{
    private readonly IAnomalyCachingService _anomalyCachingService;
    private readonly IAppSettingsCachingService _appSettingsCachingService;
    private readonly IAuthorCachingService _authorCachingService;
    private readonly IBlockCachingService _blockCachingService;
    private readonly ICategoryCachingService _categoryCachingService;
    private readonly IMenuCachingService _menuCachingService;
    private readonly INewsCachingService _newsCachingService;
    private readonly INewsPhotoTypeCachingService _newsPhotoTypeCachingService;
    private readonly INewsStatusCachingService _newsStatusCachingService;
    private readonly INewsTypeCachingService _newsTypeCachingService;
    private readonly IPageCachingService _pageCachingService;
    private readonly IPhotoEditorCachingService _photoEditorCachingService;
    private readonly IPromptCachingService _promptCachingService;
    private readonly ISourceCachingService _sourceCachingService;
    private readonly ISystemLogCachingService _systemLogCachingService;
    private readonly ITagCachingService _tagCachingService;
    private readonly IUserCachingService _userCachingService;
    private readonly IXmlFeedCachingService _xmlFeedCachingService;
    private readonly IStringLocalizer<CommonResource> _localizer;

    public CachesController(IAnomalyCachingService anomalyCachingService, IAppSettingsCachingService appSettingsCachingService, IAuthorCachingService authorCachingService, IBlockCachingService blockCachingService, ICategoryCachingService categoryCachingService, IMenuCachingService menuCachingService, INewsCachingService newsCachingService, INewsPhotoTypeCachingService newsPhotoTypeCachingService, INewsStatusCachingService newsStatusCachingService, INewsTypeCachingService newsTypeCachingService, IPageCachingService pageCachingService, IPhotoEditorCachingService photoEditorCachingService, IPromptCachingService promptCachingService, ISourceCachingService sourceCachingService, ISystemLogCachingService systemLogCachingService, ITagCachingService tagCachingService, IUserCachingService userCachingService, IStringLocalizer<CommonResource> localizer, IXmlFeedCachingService xmlFeedCachingService)
    {
        _xmlFeedCachingService = xmlFeedCachingService;
        _anomalyCachingService = anomalyCachingService;
        _appSettingsCachingService = appSettingsCachingService;
        _authorCachingService = authorCachingService;
        _blockCachingService = blockCachingService;
        _categoryCachingService = categoryCachingService;
        _menuCachingService = menuCachingService;
        _newsCachingService = newsCachingService;
        _newsPhotoTypeCachingService = newsPhotoTypeCachingService;
        _newsStatusCachingService = newsStatusCachingService;
        _newsTypeCachingService = newsTypeCachingService;
        _pageCachingService = pageCachingService;
        _photoEditorCachingService = photoEditorCachingService;
        _promptCachingService = promptCachingService;
        _sourceCachingService = sourceCachingService;
        _systemLogCachingService = systemLogCachingService;
        _tagCachingService = tagCachingService;
        _userCachingService = userCachingService;
        _localizer = localizer;
    }

    public IActionResult List()
    {
        return View();
    }

    public async Task<IActionResult> Refresh(string cacheName)
    {
        ServiceResult result;
        
        switch (cacheName)
        {
            case "anomaly":
                result = await _anomalyCachingService.RefreshAll();
                return Ok(result);
            case "appsettings":
                result = await _appSettingsCachingService.RefreshAll();
                return Ok(result);
            case "author":
                result = await _authorCachingService.RefreshAll();
                return Ok(result);
            case "block":
                result = await _blockCachingService.RefreshAll();
                return Ok(result);
            case "category":
                result = await _categoryCachingService.RefreshAll();
                return Ok(result);
            case "menu":
                result = await _menuCachingService.RefreshAll();
                return Ok(result);
            case "newsPhotoTypes":
                result = await _newsPhotoTypeCachingService.RefreshAll();
                return Ok(result);
            case "newsStatus":
                result = await _newsStatusCachingService.RefreshAll();
                return Ok(result);
            case "newsType":
                result = await _newsTypeCachingService.RefreshAll();
                return Ok(result);
            case "page":
                result = await _pageCachingService.RefreshAll();
                return Ok(result);
            case "prompt":
                result = await _promptCachingService.RefreshAll();
                return Ok(result);
            case "source":
                result = await _sourceCachingService.RefreshAll();
                return Ok(result);
            case "systemlog":
                result = await _systemLogCachingService.RefreshAll();
                return Ok(result);
            case "tag":
                result = await _tagCachingService.RefreshAll();
                return Ok(result);
            case "user":
                result = await _userCachingService.RefreshAll();
                return Ok(result);
			case "news":
				result = await _newsCachingService.RefreshAll();
				return Ok(result);
			case "rss":
				result = await _xmlFeedCachingService.RefreshAllRSS();
				return Ok(result);
			case "google-ai-rss":
				result = await _xmlFeedCachingService.RefreshGoogleAiRSS();
				return Ok(result);
			case "sitemaps":
				result = await _xmlFeedCachingService.RefreshAllSiteMaps();
				return Ok(result);			
			default:
                var errorResult = new ServiceResult();
                errorResult.SetError(_localizer["RecordNotFound"]);
                Message(errorResult);
                return RedirectToAction("List");
        }
    }
}