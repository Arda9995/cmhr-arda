using AvvaMobile.Core;
using AvvaMobile.Core.DataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quark.Business.Common.PaymentServices.Models.Requests;
using Quark.CMS.Business.Constants;
using Quark.CMS.Business.Interfaces;
using Quark.CMS.Business.Models.Requests.PaymentManagement;

namespace Quark.CMS.Controllers;

[Authorize(Roles = SecurityClaimKeys.PAYMENT_MANAGEMENTS)]
public class PaymentManagementController : BaseController
{
    private readonly IPaymentManagementService _paymentManagementService;
    public PaymentManagementController(IPaymentManagementService paymentManagementService)
    {
        _paymentManagementService = paymentManagementService;
    }
    
    [HttpGet]
    public IActionResult Products()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult CreateProduct()
    {
        var request = new CreateCumhuriyetProductRequest
        {
            ConversationId = Guid.NewGuid().ToString()
        };
        return View(request);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateCumhuriyetProductRequest request)
    {
        var response = await _paymentManagementService.CreateProductAsync(request);
        return RedirectToAction("Products", "PaymentManagement");
    }
    
    [HttpGet]
    public IActionResult Plans()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult CreatePlan()
    {
        var products = _paymentManagementService.GetProductsSelectListItemAsync().Result;
        var request = new CreateCumhuriyetPlanRequest
        {
            ConversationId = Guid.NewGuid().ToString(),
            ProductReferenceCodes = products.Data,
        };
        return View(request);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePlan(CreateCumhuriyetPlanRequest request)
    {
        var response = await _paymentManagementService.CreatePlanAsync(request);
        return RedirectToAction("Plans", "PaymentManagement");
    }
    
    [HttpGet]
    public async Task<IActionResult> UpdatePlan(int id)
    {
        var response = await _paymentManagementService.UpdatePlanAsync(id);
        return View(response.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdatePlan(UpdateCumhuriyetPlanRequest request)
    {
        var response = await _paymentManagementService.UpdatePlanAsync(request);
        return RedirectToAction("Plans", "PaymentManagement");
    }
    
    [HttpGet]
    public IActionResult Subscriptions()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> GetAllSubscriptionsPaged()
    {
        var result = await _paymentManagementService.GetAllSubscriptionsPagedAsync(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> SubscriptionDetail(int id)
    {
        var result = await _paymentManagementService.GetSubscriptionDetail(id);
        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetAllPlansPaged()
    {
        var result = await _paymentManagementService.GetAllPlansPagedAsync(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> GetAllProductsPaged()
    {
        var result = await _paymentManagementService.GetAllProductsPagedAsync(new BaseDataTableRequest(Request));
        Message(result);
        return Json(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> SubscriptionsUpgrade(int id)
    {
        var result = await _paymentManagementService.GetSubscriptionsUpgradeRequestAsync(id);
        return View(result.Data);
    }
    
    [HttpPost]
    public async Task<IActionResult> SubscriptionsUpgrade(SubscriptionsUpgradeRequest request)
    {
        var result = await _paymentManagementService.UpgradeSubscriptionAsync(request);
        return View(result.Data);
    }
    
}