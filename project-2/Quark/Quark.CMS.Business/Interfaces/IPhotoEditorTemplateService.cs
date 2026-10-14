using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.PhotoEditor;
using Quark.CMS.Business.Models.Responses.PhotoEditorResponses;

namespace Quark.CMS.Business.Interfaces;

public interface IPhotoEditorTemplateService
{
    Task<BaseDataTableResponse<List<ListTemplateItemResponse>>> GetAllPaged(BaseDataTableRequest tableRequest);
    Task<ServiceResult<CreateTemplateResponse>> CreateTemplateAsync(CreateTemplateRequest request);
    Task<ServiceResult<PrepareEditResponse>> PrepareEditAsync(PrepareEditRequest request);
    Task<ServiceResult> DeleteTemplateAsync(int id);
    Task<ServiceResult<UpdateTemplateRequest>> UpdateTemplateAsync(int id);
    Task<ServiceResult<UpdateTemplateResponse>> UpdateTemplateAsync(UpdateTemplateRequest request);
}