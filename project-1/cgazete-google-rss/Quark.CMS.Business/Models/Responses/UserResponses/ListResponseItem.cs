namespace Quark.CMS.Business.Models.Responses.UserResponses
{
    public class ListResponseItem
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public bool IsEnabled { get; set; }
        public string IsEnabledText { get; set; }
        public string RoleName { get; set; }
    }
}