namespace Quark.CMS.Business.Models.Responses.AgencyNewsResponses
{
    public class ListResponseItem
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public string AgencyName { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveText { get; set; }
        public string Date { get; set; }
    }
}
