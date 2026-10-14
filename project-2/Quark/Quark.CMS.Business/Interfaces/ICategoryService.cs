using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.CategoryRequests;
using Quark.CMS.Business.Models.Responses.CategoryResponses;

namespace Quark.CMS.Business.Interfaces;

public interface ICategoryService
{
    ServiceResult<CreateUpdateRequest> Create(int? currentId);
    Task<ServiceResult<CreateUpdateRequest>> Create(CreateUpdateRequest request);
    Task<ServiceResult<CreateUpdateRequest>> Update(int currentId);
    Task<ServiceResult<CreateUpdateRequest>> Update(CreateUpdateRequest request);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<CategoryDetailsResponse>> Details(int id);
    Task<BaseDataTableResponse<List<DetailsCategoriesNewsResponseItem>>> GetNewsPaged(BaseDataTableRequest tableRequest, GetCategoryFiltersRequest request);
    Task<ServiceResult<List<GetCategoryTreeItem>>> GetCategoriesTree(int? parent, bool onlyEnableds);
    Task<ServiceResult> ChangeCategoryStatus(int ID, bool isEnabled);

}