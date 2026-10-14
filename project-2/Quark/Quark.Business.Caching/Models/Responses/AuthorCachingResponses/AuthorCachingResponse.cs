
using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Business.Caching.Models.Responses.AuthorCachingResponses
{
    public class AuthorCachingResponse
    {
        public int ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public bool IsEnabled { get; set; }
        public int? TotalNewsCount { get; set; } = 0;
        public string Description { get; set; }
        public bool ShowInHomePage { get; set; }
        public string Url { get; set; }
        public string LastCreatedNewsHeadline { get; set; }
        public string LastCreatedNewsSlug { get; set; }
        public int LastCreatedNewsID { get; set; }
        public string AuthorSocialX { get; set; }
        public string AuthorSocialInstagram { get; set; }
        public string AuthorSocialFacebook { get; set; }
        public string AuthorSocialLinkedIn { get; set; }
        public string AuthorSocialMail { get; set; }
    }
}
