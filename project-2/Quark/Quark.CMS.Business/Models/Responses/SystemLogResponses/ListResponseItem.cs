namespace Quark.CMS.Business.Models.Responses.SystemLogResponses;

public class ListResponseItem
{
    public int ID { get; set; }
    public int? UserID { get; set; }
    public string Credential { get; set; }
    public string UserName { get; set; }
    public string IpAddress { get; set; }
    public string CreatedDate { get; set; }
    public bool IsSuccess { get; set; }
    public string IsSuccessText { get; set; }
    public string Message { get; set; }
    public string LogType { get; set; }
    public string Url { get; set; }
    public long? ObjectID { get; set; }
}