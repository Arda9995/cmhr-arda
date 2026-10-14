
namespace Quark.CMS.Business.Models.Requests.AuthorRequests
{
    public class DetailsAuthorRequest
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public bool IsEnabled{ get; set; }
        public string ClichePhoto { get; set; }
    }
}
