namespace Quark.CMS.Business.Models.Responses.MemberResponses
{
    public class ListMemberCreditCardsResponse
    {
        public int ID { get; set; }
        public string CreatedDate { get; set; }
        public string Name { get; set; }
        public string CardHolderName { get; set; }
        public string CardNo { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string CVV { get; set; }
    }
}