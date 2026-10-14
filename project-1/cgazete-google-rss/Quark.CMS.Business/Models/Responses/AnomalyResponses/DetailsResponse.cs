namespace Quark.CMS.Business.Models.Responses.AnomalyResponses;

public class DetailsResponse
{
    public int ID { get; set; }
    public string DataURL { get; set; }
    public string DataType { get; set; }
    public string Issue { get; set; }
    public string CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string Description { get; set; }
    public bool IsSolved { get; set; }
}