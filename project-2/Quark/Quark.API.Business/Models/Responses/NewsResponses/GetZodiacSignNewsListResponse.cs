using Quark.API.Business.Models.Responses.PageResponses;

namespace Quark.API.Business.Models.Responses.NewsResponses
{
    public class GetZodiacSignNewsListResponse
    {
        public List<ZodiacSignBlock> ZodiacSignItems { get; set; }
        public string AstrologerName { get; set; }
        public string AstrologerImageURL { get; set; }
        public string DateStringForZodiacSignDateRange { get; set; }
    }
}
