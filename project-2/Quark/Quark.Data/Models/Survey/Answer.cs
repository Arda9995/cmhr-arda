using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Survey;

[Table("Survey.Answers")]
public class Answer
{
    public int ID { get; set; }
    public int SurveyID { get; set; }
    public string UserID { get; set; }
    public int Option { get; set; }
    public string IP { get; set; }
    public DateTime CreatedDate { get; set; }
}