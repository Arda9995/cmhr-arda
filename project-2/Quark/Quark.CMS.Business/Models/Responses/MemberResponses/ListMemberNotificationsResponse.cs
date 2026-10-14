namespace Quark.CMS.Business.Models.Responses.MemberResponses
{
    public class ListMemberNotificationsResponse
    {
        public int? SenderID { get; set; }
        public string SenderName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public bool IsRead { get; set; }
        public string IsReadText { get; set; }
        public string CreatedDate { get; set; }
    }
}