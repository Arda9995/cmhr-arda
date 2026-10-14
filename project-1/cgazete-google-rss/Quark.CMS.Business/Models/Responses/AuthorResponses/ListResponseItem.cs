
namespace Quark.CMS.Business.Models.Responses.AuthorResponses
{
    public class ListResponseItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public bool IsEnabled { get; set; }
        public string IsEnabledText { get; set; }
        public string NameInitials { get; set; }

    }
}
