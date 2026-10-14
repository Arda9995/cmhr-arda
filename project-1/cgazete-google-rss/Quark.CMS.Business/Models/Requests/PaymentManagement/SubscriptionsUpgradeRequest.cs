using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quark.CMS.Business.Models.Requests.PaymentManagement;

public class SubscriptionsUpgradeRequest
{
    public int CurrentPlanID { get; set; }
    public string CurrentPlanName { get; set; }
    public List<SelectListItem> PaymentPlans { get; set; }
    public int SelectedPlanID { get; set; }
}