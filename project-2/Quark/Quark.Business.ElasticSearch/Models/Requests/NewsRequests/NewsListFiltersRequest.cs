namespace Quark.Business.ElasticSearch.Models.Requests.NewsRequests
{
    public class NewsListFiltersRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string Sort { get; set; } = "iD";
        public string Direction { get; set; } = "asc";
        public string SearchValue { get; set; }
        public List<int> TagIDs { get; set; }
        public int AuthorID { get; set; }
        public int NewsTypeID { get; set; }
        public int CategoryID { get; set; }
        public List<int> SecondaryCategoryIDs { get; set; }
    }
}
