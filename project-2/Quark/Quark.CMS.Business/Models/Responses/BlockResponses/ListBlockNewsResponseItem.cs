namespace Quark.CMS.Business.Models.Responses.BlockResponses;

public class ListBlockNewsResponseItem
{
    public long ID { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public int ViewCount { get; set; }
    public string PublishDate { get; set; }
    public string PhotoUrl { get; set; }
    public bool InUse { get; set; }
    public List<string> InUseBlockList { get; set; }
    
}
