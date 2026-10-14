namespace Quark.Business.Caching.Models.Responses.AuthorCachingResponses
{
    public class GetAuthorLastColumnArticlesCacheResponse
    {
        public int ID { get; set; }
        public bool IsEnabled { get; set; }
        public string Photo { get; set; }
        public string FullName { get; set; }
        public string Slug { get; set; }
        public string SocialX { get; set; }
        public string SocialInstagram { get; set; }
        public string SocialFacebook { get; set; }
        public string SocialLinkedIn { get; set; }
        public string SocialMail { get; set; }
        public List<AuthorsLastColumnArticlesCacheResponse> AuthorsLastColumnArticles { get; set; } = new List<AuthorsLastColumnArticlesCacheResponse>();

    }
}
