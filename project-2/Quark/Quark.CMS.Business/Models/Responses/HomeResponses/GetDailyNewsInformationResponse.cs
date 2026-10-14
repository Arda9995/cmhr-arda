namespace Quark.CMS.Business.Models.Responses.HomeResponses
{
    public class GetDailyNewsInformationResponse
    {
        public int RelatedNewsCount { get; set; }
        public int WithoutRelatedNewsCount { get; set; }
        public int TaggedNewsCount { get; set; }
        public int WithoutTaggedNewsCount { get; set; }
    }
}
