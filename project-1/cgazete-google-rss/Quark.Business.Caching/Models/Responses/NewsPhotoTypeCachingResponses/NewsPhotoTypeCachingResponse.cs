namespace Quark.Business.Caching.Models.Responses.NewsPhotoTypeCachingResponses
{
    public class NewsPhotoTypeCachingResponse
    {
        public int ID { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsRequired { get; set; }
        public bool IsEnabled { get; set; }
        public int Order { get; set; }
    }
}
