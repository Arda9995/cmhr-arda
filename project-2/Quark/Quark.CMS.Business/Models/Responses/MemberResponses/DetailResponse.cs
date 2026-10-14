namespace Quark.CMS.Business.Models.Responses.MemberResponses
{
    public class DetailResponse
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public bool IsEnabled { get; set; }
    }
}