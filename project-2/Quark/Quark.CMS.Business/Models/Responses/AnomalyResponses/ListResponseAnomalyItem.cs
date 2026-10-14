namespace Quark.CMS.Business.Models.Responses.AnomalyResponses;

public class ListResponseAnomalyItem
{
    public int ID { get; set; }
    public string DataURL { get; set; }
    public string DataType { get; set; }
    public string Issue { get; set; }
    public string IsSolvedText { get; set; }
    public bool IsSolved { get; set; }
    public string CreatedDate { get; set; }
}