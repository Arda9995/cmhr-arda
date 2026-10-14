namespace Quark.CMS.Business.Models.Responses.AgencyNewsResponses
{
    public class AgencyNewsDetailResponse
    {
        public int ID { get; set; }
        public string AgencyName { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
    }
}