using System.ComponentModel.DataAnnotations.Schema;

namespace Quark.Data.Models.Survey
{
    [Table("Survey.SurveyOptions")]
    public class SurveyOptions
    {
        public int ID { get; set; }
        public long NewsVersionID { get; set; }
        public string Option { get; set; }
        public int Order { get; set; }
        public int SurveyID { get; set; }
        public long AnswerCount { get; set; }
    }
}
