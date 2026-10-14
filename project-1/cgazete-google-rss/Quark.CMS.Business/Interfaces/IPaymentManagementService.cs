using AvvaMobile.Core.Business;
using AvvaMobile.Core.DataTable;
using Iyzipay.Request.V2.Subscription;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quark.Business.Common.PaymentServices.Models.Requests;
using Quark.CMS.Business.Models.Requests.PaymentManagement;
using Quark.CMS.Business.Models.Responses.PaymentManagementResponses;

namespace Quark.CMS.Business.Interfaces;

public interface IPaymentManagementService
{
    Task<ServiceResult> CreateProductAsync(CreateCumhuriyetProductRequest request);
    Task<ServiceResult> CreatePlanAsync(CreateCumhuriyetPlanRequest request);
    Task<ServiceResult<List<SelectListItem>>> GetProductsSelectListItemAsync();
    Task<BaseDataTableResponse<List<ListResponsePaymentPlansItem>>> GetAllPlansPagedAsync(
        BaseDataTableRequest tableRequest);
    Task<BaseDataTableResponse<List<ListResponsePaymentProductsItem>>> GetAllProductsPagedAsync(
        BaseDataTableRequest tableRequest);
    Task<BaseDataTableResponse<List<ListResponseSubscriptionItem>>> GetAllSubscriptionsPagedAsync(
        BaseDataTableRequest tableRequest);
    Task<ServiceResult<SubscriptionsUpgradeRequest>> GetSubscriptionsUpgradeRequestAsync(int paymentPlanId);
    Task<ServiceResult<SubscriptionsUpgradeRequest>> UpgradeSubscriptionAsync(
        SubscriptionsUpgradeRequest request);
    Task<ServiceResult<UpdateCumhuriyetPlanRequest>> UpdatePlanAsync(int id);
    Task<ServiceResult<UpdateCumhuriyetPlanRequest>> UpdatePlanAsync(UpdateCumhuriyetPlanRequest request);
    Task<ServiceResult> ActivateSubscriptionAsync(string referenceCode);
    Task<ServiceResult<SubscriptionDetailResponse>> GetSubscriptionDetail(int memberId);
    
}