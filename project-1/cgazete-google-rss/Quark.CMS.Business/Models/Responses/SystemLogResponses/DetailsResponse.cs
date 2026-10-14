namespace Quark.CMS.Business.Models.Responses.SystemLogResponses;

public class DetailsResponse
{
    public int ID { get; set; }
    public string UserName { get; set; }
    public string Credentials { get; set; }
    public string IpAdress { get; set; }
    public string CreatedDate { get; set; }
    public string Message { get; set; }
    public string LogType { get; set; }
    public int LogTypeID { get; set; }
    public string Url { get; set; }
}