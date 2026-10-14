using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.DataTable;
using AvvaMobile.Core.Extensions;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Quark.Business.Caching.Interfaces;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.PhotoEditor;
using Quark.CMS.Business.Models.Responses.PhotoEditorResponses;
using Quark.Data;
using Quark.Data.Models.PhotoEditor;
using Quark.Resources;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;
using Color = SixLabors.ImageSharp.Color;

namespace Quark.CMS.Business.Services;

public class PhotoEditorTemplateTemplateService : BaseService, IPhotoEditorTemplateService
{
    IPhotoEditorCachingService _photoEditorCachingService;
    public PhotoEditorTemplateTemplateService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, IPhotoEditorCachingService photoEditorCachingService) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _photoEditorCachingService = photoEditorCachingService;
    }

    public async Task<BaseDataTableResponse<List<ListTemplateItemResponse>>> GetAllPaged(BaseDataTableRequest tableRequest)
    {
        var response = new BaseDataTableResponse<List<ListTemplateItemResponse>>();

        response.RecordsTotal = await _context.Templates.CountAsync(x => x.IsEnabled);

        var query = (from template in _context.Templates
                     join user in _context.Users on template.CreatedBy equals user.ID
                     where template.IsEnabled
                     select new
                     {
                         template = template,
                         user = user
                     }).AsQueryable();

        if (!string.IsNullOrEmpty(tableRequest.SearchValue))
        {
            query = query.Where(
                x => x.template.Name.ToUpper().Contains(tableRequest.SearchValue.ToUpper())
            );
        }

        if (tableRequest.PageSize > 100)
        {
            tableRequest.PageSize = 100;
        }

        query = query.SortSkipTake(tableRequest);

        response.Data = await query.OrderBy(x => x.template.Name)
            .Select(x => new ListTemplateItemResponse
            {
                Id = x.template.ID,
                Name = x.template.Name,
                CreatedUsername = x.user.Firstname + " " + x.user.Lastname,
                CreatedDatetime = x.template.CreatedDatetime.ToString("G")
            }).ToListAsync();
        return response;
    }

    public async Task<ServiceResult<CreateTemplateResponse>> CreateTemplateAsync(CreateTemplateRequest request)
    {
        var respone = new ServiceResult<CreateTemplateResponse>();

        var isExist = await _context.Templates.Where(x => x.Name == request.Name).FirstOrDefaultAsync();

        if (isExist != null)
        {
            respone.SetError(_localizer["RecordAlreadyExist"]);
            return respone;
        }

        var template = new Template
        {
            Name = request.Name,
            CreatedBy = CurrentUserID,
            CreatedDatetime = DateTime.Now,
            TextFontSize = request.FontSize,
            BackgroundOpacity = request.Opacity,
            BackgroundColor = request.BackgroundColor,
            TextColor = request.TextColor,
            IsEnabled = true
        };

        await _context.Templates.AddAsync(template);
        await _context.SaveChangesAsync();

        return respone;
    }

    public async Task<ServiceResult<PrepareEditResponse>> PrepareEditAsync(PrepareEditRequest request)
    {
        #region variables

        var response = new ServiceResult<PrepareEditResponse>();
        var vocab = "SON DAKİKA! SON DAKİKA! SON DAKİKA! SON DAKİKA!";
        var opacity = request.Opacity;
        Color backgroundColor;
        Color textColor;

        #endregion

        #region Colors

        try
        {
            backgroundColor = Color.ParseHex(request.BackgroundColor ?? "#FFFFFF");
        }
        catch (Exception e)
        {
            backgroundColor = Color.ParseHex("#FFFFFF");
        }

        try
        {
            textColor = Color.ParseHex(request.TextColor ?? "#000000");
        }
        catch (Exception e)
        {
            textColor = Color.ParseHex("#000000");
        }

        #endregion

        var font = SystemFonts.CreateFont("Arial", request.TextFontSize > 0 ? request.TextFontSize : 100);
        var textSize = TextMeasurer.MeasureSize(vocab, new TextOptions(font));

        #region Create Image

        var resultImage = LoadImageWithPath("wwwroot/img/ataturk.jpg");
        var backgroundImage = new Image<Rgba32>(resultImage.Width, (int)((textSize.Height > 0 ? textSize.Height : 1) * 2), backgroundColor);

        #endregion

        #region Locations

        var bgLocationX = 0;
        var bgLocationY = resultImage.Height - backgroundImage.Height;

        var textLocationX = bgLocationX + 20;
        var textLocationY = bgLocationY + ((backgroundImage.Height - textSize.Height) / 2);

        #endregion

        #region Mutate Process

        resultImage.Mutate(x =>
            x.DrawImage(backgroundImage, new Point(bgLocationX, bgLocationY),
                opacity / 100f));

        resultImage.Mutate(x => x.DrawText(
            vocab,
            font,
            Brushes.Solid(textColor),
            new Point((int)textLocationX, (int)textLocationY)));

        #endregion

        var prepareEditResponse = new PrepareEditResponse
        {
            Base64Image = ImageToBase64(resultImage)
        };

        response.Data = prepareEditResponse;
        return response;
    }

    public async Task<ServiceResult> DeleteTemplateAsync(int id)
    {
        var result = new ServiceResult();

        var template = await _context.Templates.FirstOrDefaultAsync(x => x.ID == id);
        if (template is null)
        {
            result.SetError(_localizer["RecordNotFound"]);
            return result;
        }

        _context.Templates.Remove(template);
        await _context.SaveChangesAsync();

        result.SetSuccess(_localizer["RecordDeleted"]);
        return result;
    }

    public async Task<ServiceResult<UpdateTemplateRequest>> UpdateTemplateAsync(int id)
    {
        var response = new ServiceResult<UpdateTemplateRequest>();

        var template = _context.Templates.FirstOrDefault(x => x.ID == id);

        if (template is null)
        {
            response.SetError(_localizer["RecordNotFound"]);
            return response;
        }

        var templateImageValues = new PrepareEditRequest
        {
            BackgroundColor = template.BackgroundColor,
            TextColor = template.TextColor,
            TextFontSize = template.TextFontSize,
            Opacity = template.BackgroundOpacity
        };

        var templateImage = await PrepareEditAsync(templateImageValues);

        var updateTemplateRequest = new UpdateTemplateRequest
        {
            Id = template.ID,
            Name = template.Name,
            FontSize = template.TextFontSize,
            Opacity = template.BackgroundOpacity,
            BackgroundColor = template.BackgroundColor,
            TextColor = template.TextColor,
            Base64Image = "data:image/png;base64," + templateImage.Data.Base64Image
        };

        response.Data = updateTemplateRequest;
        return response;
    }

    public async Task<ServiceResult<UpdateTemplateResponse>> UpdateTemplateAsync(UpdateTemplateRequest request)
    {
        var response = new ServiceResult<UpdateTemplateResponse>();

        var isExist = _context.Templates.FirstOrDefault(x => x.ID != request.Id && x.Name == request.Name);

        if (isExist != null)
        {
            response.SetError(_localizer["RecordAlreadyExist"]);
            return response;
        }

        var template = _context.Templates.FirstOrDefault(x => x.ID == request.Id);

        template.Name = request.Name;
        template.TextFontSize = request.FontSize;
        template.BackgroundOpacity = request.Opacity;
        template.BackgroundColor = request.BackgroundColor;
        template.TextColor = request.TextColor;

        _context.Templates.Update(template);
        await _context.SaveChangesAsync();

        return response;
    }

    #region Helpers

    private string ImageToBase64(Image image)
    {
        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        var imageBytes = ms.ToArray();
        var base64String = Convert.ToBase64String(imageBytes);

        ms.Close();

        return base64String;
    }
    private Image Base64ToImage(string base64String)
    {
        var imageBytes = Convert.FromBase64String(base64String);
        var image = Image.Load(imageBytes);
        return image;
    }
    private Image LoadImageWithPath(string path)
    {
        var imagePath = Path.Combine(path);
        return Image.Load(imagePath);
    }

    #endregion


}