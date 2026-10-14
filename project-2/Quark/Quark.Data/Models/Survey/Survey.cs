using System.ComponentModel.DataAnnotations.Schema;


namespace Quark.Data.Models.Survey;

[Table("Survey.Surveys")]
public class Survey
{
    public long NewsVersionID { get; set; }
    public int ID { get; set; }
    public string Question { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StartDate { get; set; }
    public int CreatedBy { get; set; }
    
}