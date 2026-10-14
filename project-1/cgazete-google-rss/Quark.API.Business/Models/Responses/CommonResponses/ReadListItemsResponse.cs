using System.Collections.Generic;

namespace Quark.API.Business.Models.Responses.CommonResponses
{
    public class ReadListItemsResponse
    {
        public List<ReadListItem> Items { get; set; } = new List<ReadListItem>();
        public bool HasMoreItems { get; set; }
    }
} 