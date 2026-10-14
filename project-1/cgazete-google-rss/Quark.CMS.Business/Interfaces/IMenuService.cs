using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Quark.CMS.Business.Models.Requests.MenuRequests;
using Quark.CMS.Business.Models.Responses.MenuResponses;

namespace Quark.CMS.Business.Interfaces;

public interface IMenuService
{

    Task<ServiceResult> ChangeMenuStatus(int ID, bool isEnabled);
    Task<ServiceResult<CreateUpdateRequest>> Create(CreateUpdateRequest request);
    Task<ServiceResult<CreateUpdateRequest>> CreateParentMenu(CreateUpdateRequest request);
    ServiceResult<CreateUpdateRequest> Create(int menuId, int? currentId);
    ServiceResult<CreateUpdateRequest> CreateParentMenu();
    Task<ServiceResult> Delete(int ID);
    Task<ServiceResult<List<GetMenuTreeItem>>> GetMenusTree(int? parent, bool onlyEnableds);
    Task<ServiceResult<CreateUpdateRequest>> Update(int menuId, int currentId);
    Task<ServiceResult<CreateUpdateRequest>> Update(CreateUpdateRequest request);
    Task<BaseDataTableResponse<List<ListResponseMenuItem>>> GetAllPaged(BaseDataTableRequest tableRequest);
    Task<ServiceResult<MenuDetailsRequest>> Details(int id);
    Task<ServiceResult<CreateUpdateRequest>> UpdateParent(int ID);
    Task<ServiceResult<CreateUpdateRequest>> UpdateParent(CreateUpdateRequest request);
}