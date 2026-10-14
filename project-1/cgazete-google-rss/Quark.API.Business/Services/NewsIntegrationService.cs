using System.Text;
using AvvaMobile.Core.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.API.Business.Interfaces;
using Quark.API.Business.Models.Responses.NewsManagementResponses;
using Quark.Business.Common.BaseServices;
using Quark.Data;
using Quark.Resources;
using Spire.Doc;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.API.Business.Services;

public class NewsIntegrationService(
    IHttpContextAccessor httpContext,
    QuarkDataContext context,
    IStringLocalizer<CommonResource> localizer,
    AppSettingsKeys appSettingsKeys,
    ICacheManager cache)
    : BaseService(httpContext, context, localizer, appSettingsKeys, cache), INewsIntegrationService
{
    public async Task<GetPageCategoriesResponse> GetPageCategoriesAsync()
    {
        var response = new GetPageCategoriesResponse()
        {
            Ok = true,
            Data = new List<GetPageCategory>()
        };

        var pages = await _context.NewsPages.Select(x => new GetPageCategory
        {
            Id = x.ID,
            Name = x.Name
        }).ToListAsync();

        if (!pages.Any())
        {
            response.Ok = false;
            return response;
        }

        response.Data = pages;

        return response;
    }

    public async Task<GetListStoriesResponse> GetListStoriesAsync(long? newsId)
    {
        var response = new GetListStoriesResponse()
        {
            Ok = true,
            Data = new List<ListStoryItem>()
        };

        var query = await (from pn in _context.PrintingNews
                join c in _context.Categories on pn.CategoryID equals c.ID
                where (newsId == null || pn.NewsID == newsId)
                orderby pn.CreatedDate descending
                select new ListStoryItem
                {
                    Id = pn.ID,
                    NewsTitle = pn.Title,
                    NewsCategory = c.Name
                }
            ).Take(100).ToListAsync();

        if (!query.Any())
        {
            response.Ok = false;
            return response;
        }

        response.Data = query;
        return response;
    }

    public async Task<GetStoryResponse> GetStoryAsync(long newsId)
    {
        var response = new GetStoryResponse()
        {
            Ok = true,
            Data = null
        };

        var query = await (from pn in _context.PrintingNews
                join c in _context.Categories on pn.CategoryID equals c.ID
                where (pn.NewsID == newsId)
                orderby pn.CreatedDate descending
                select new
                {
                    Id = pn.ID,
                    NewsTitle = pn.Title,
                    NewsCategory = c.Name,
                    Content = pn.Content
                }
            ).FirstOrDefaultAsync();

        if (query == null)
        {
            response.Ok = false;
            return response;
        }

        var storyContentRtf = ConvertHtmlToRtf(query.Content);
        var queryResult = new StoryItem()
        {
            Id = query.Id,
            NewsTitle = query.NewsTitle,
            NewsCategory = query.NewsCategory,
            Content = storyContentRtf
        };
        response.Data = queryResult;
        return response;
    }
    
    private string ConvertHtmlToRtf(string htmlContent)
    {
        Document document = new Document();
        byte[] htmlBytes = Encoding.UTF8.GetBytes(htmlContent);
        using (MemoryStream htmlStream = new MemoryStream(htmlBytes))
        {
            document.LoadFromStream(htmlStream, FileFormat.Html);
        }
        
        using (MemoryStream rtfStream = new MemoryStream())
        {
            document.SaveToStream(rtfStream, FileFormat.Rtf);
            return Encoding.ASCII.GetString(rtfStream.ToArray());
        }
    }
}