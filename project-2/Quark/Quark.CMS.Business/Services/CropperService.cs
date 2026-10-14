using AvvaMobile.Core.Business;
using AvvaMobile.Core.Caching;
using AvvaMobile.Core.Utilities.FtpUpload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Quark.Business.Common.BaseServices;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.CropperRequests;
using Quark.CMS.Business.Models.Responses.CropperResponses;
using Quark.Data;
using Quark.Resources;
using AppSettingsKeys = Quark.Business.Common.Caching.AppSettingsKeys;

namespace Quark.CMS.Business.Services;

public class CropperService : BaseService, ICropperService
{
    private readonly FileUpload _fileUpload;

    public CropperService(IHttpContextAccessor httpContext, QuarkDataContext context, IStringLocalizer<CommonResource> localizer, AppSettingsKeys appSettingsKeys, ICacheManager cache, FileUpload fileUpload) : base(httpContext, context, localizer, appSettingsKeys, cache)
    {
        _fileUpload = fileUpload;
    }

    public ServiceResult<CropperPhotoResponse> EditImage(Base64ImageRequest request)
    {
        var result = new ServiceResult<CropperPhotoResponse>();

        if (string.IsNullOrEmpty(request.Base64Image))
        {
            result.SetError("Hata");
            return result;
        }

        var base64Data = request.Base64Image;
        var base64Content = "";
        var prefix = $"data:{request.UploadedImageType};base64,";

        if (base64Data.StartsWith(prefix))
        {
            string[] parts = base64Data.Split(',');
            if (parts.Length == 2)
            {
                string imageType = parts[0].Split(';')[0].Split(':')[1];
                base64Content = parts[1];
            }
        }

        byte[] imageBytes = Convert.FromBase64String(base64Content);

        using (MemoryStream ms = new MemoryStream(imageBytes))
        {
            var file = new FormFile(ms, 0, ms.Length, "file", request.FileName);

            var uploadResult = _fileUpload.Upload(file, _appSettingsKeys.CDN_MediaGalleryFolder);

            if (uploadResult != null)
            {
                result.Data = new CropperPhotoResponse()
                {
                    PhotoURL = _appSettingsKeys.CDN_BaseUrl + _appSettingsKeys.CDN_MediaGalleryFolder + uploadResult.FileName
                };
            }
        }

        return result;
    }
}