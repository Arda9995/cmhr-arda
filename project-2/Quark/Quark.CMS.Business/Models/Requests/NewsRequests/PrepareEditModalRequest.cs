namespace Quark.CMS.Business.Models.Requests.NewsRequests;

public class PrepareEditModalRequest
{
    public string PhotoUrl { get; set; }
    public int LocationId { get; set; }
    public string Text1 { get; set; }
    public string Text2 { get; set; }
    public int Thema1Id { get; set; }
    public int Thema2Id { get; set; }
}