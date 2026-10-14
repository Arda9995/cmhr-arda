using AvvaMobile.Core.Business;
using Iyzipay.Model.V2;
using Iyzipay.Model.V2.Subscription;
using Quark.Business.Common.PaymentServices.Models.Requests;
using Quark.Business.Common.PaymentServices.Models.Responses;

namespace Quark.Business.Common.PaymentServices;

public interface IPaymentService
{
    ServiceResult<CreateProductResponse> CreateProduct(CreateCumhuriyetProductRequest request);
    Task<ServiceResult<CreateCumhuriyetPlanResponse>> CreatePaymentPlan(CreateCumhuriyetPlanRequest request);
    ServiceResult<DeleteCumhuriyetPlanResponse> DeletePaymentPlan(DeleteCumhuriyetPlanRequest request);
    Task<ServiceResult<CreateCumhuriyetSubscriptionResponse>> CreateSubscription(CreateCumhuriyetSubscriptionRequest request);
    ServiceResult<CancelCumhuriyetSubscriptionResponse> CancelSubscription(CancelCumhuriyetSubscriptionRequest request);
    Task<ServiceResult> RetrySubscriptionPayment(string referenceCode);
    ServiceResult UpgradeSubscription(UpgradeCumhuriyetSubscriptionRequest request);
    ServiceResult<SubscriptionResource> SearchSubscriptionDetail(string referenceCode);
    ServiceResult<ResponsePagingData<SubscriptionResource>> GetSubscriptionDetail(string parentReferenceCode);
    ServiceResult ActivateSubscription(string referenceCode);

}