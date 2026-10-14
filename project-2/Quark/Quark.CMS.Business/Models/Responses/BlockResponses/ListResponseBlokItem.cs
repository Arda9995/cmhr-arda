namespace Quark.CMS.Business.Models.Responses.BlokResponses;

public class ListResponseBlokItem
{
    public int ID { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public string IsEnabledText { get; set; }
    public string BlockType { get; set; }
    public string PageName { get; set; }
}