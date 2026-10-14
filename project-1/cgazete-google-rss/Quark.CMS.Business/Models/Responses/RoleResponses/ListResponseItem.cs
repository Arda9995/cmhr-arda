namespace Quark.CMS.Business.Models.Responses.RoleResponses
{
    public class ListResponseItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ActiveUserCount { get; set; }
        public int PassiveUserCount { get; set; }
        public bool IsEnabled { get; set; }
        public string IsEnabledText { get; set; }
    }
}