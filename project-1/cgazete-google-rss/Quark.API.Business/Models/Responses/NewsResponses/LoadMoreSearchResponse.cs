using System.Collections.Generic;

namespace Quark.API.Business.Models.Responses.NewsResponses
{
    public class LoadMoreSearchResponse
    {
        public List<GetNewsListSearchResponse> Items { get; set; } = new List<GetNewsListSearchResponse>();
        public bool HasMoreItems { get; set; }
        public int TotalCount { get; set; }
    }
} 