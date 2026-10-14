namespace Quark.Business.Common.PaymentServices.Models.Responses;

public class CreateProductResponse
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string ConversationId { get; set; }
    public string ReferenceCode { get; set; }
}