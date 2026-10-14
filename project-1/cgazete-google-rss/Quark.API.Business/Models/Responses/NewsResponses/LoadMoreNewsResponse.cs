using System.Collections.Generic;

namespace Quark.API.Business.Models.Responses.NewsResponses
{
    public class LoadMoreNewsResponse
    {
        public List<CategoryNews> Items { get; set; } = new List<CategoryNews>();
        public bool HasMoreItems { get; set; }
    }
} 