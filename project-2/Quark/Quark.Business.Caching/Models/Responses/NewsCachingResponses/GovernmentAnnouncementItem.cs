namespace Quark.Business.Caching.Models.Responses.NewsCachingResponses
{
    public class GovernmentAnnouncementItem
    {
        public int ID { get; set; }
        public int BoxCount { get; set; }
        public string Headline { get; set; }
        public string UpSubtitle { get; set; }
        public string Slug { get; set; }
        public string CoverPhoto { get; set; }
        public string PressNo { get; set; }
        public int OrderGroup { get; set; }
    }
    public enum GovernmentOrderGroupEnum
    {
        UnderAnaSlider = 1,
        UnderGundem,
        UnderEkonomi,
        UnderDunya,
        Remaining,
    }
}
