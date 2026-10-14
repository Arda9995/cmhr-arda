namespace Quark.CMS.Business.Models.Responses.PaymentManagementResponses;

public class ListResponsePaymentProductsItem
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ProductReferenceCode { get; set; }
    public string CreatedDate { get; set; }
    public bool IsEnabled { get; set; }
    public string IsEnabledText { get; set; }
}