using AvvaMobile.Core.Business;
using Quark.CMS.Business.Models.Requests.CropperRequests;
using Quark.CMS.Business.Models.Responses.CropperResponses;

namespace Quark.CMS.Business.Interfaces;

public interface ICropperService
{
    ServiceResult<CropperPhotoResponse> EditImage(Base64ImageRequest request);

}