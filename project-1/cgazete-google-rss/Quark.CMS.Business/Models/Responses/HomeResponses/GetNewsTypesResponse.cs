namespace Quark.CMS.Business.Models.Responses.HomeResponses
{
    public class GetNewsTypesResponse
    {
        public int TotalNewsCount { get; set; }
        public List<NewsTypeItem> NewsTypeItems { get; set; }
    }
    public class NewsTypeItem
    {
        public int NewsTypeID { get; set; }
        public int NewsCount { get; set; }
        public string NewsTypeName { get; set; }
    }
}
