using System.ComponentModel.DataAnnotations.Schema;
namespace Quark.Data.Models
{
    public class SurveyItemRequest
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CreatedBy { get; set; }
        public Dictionary<string, long> OptionWithAnswerCount { get; set; }
        public long TotalCount { get; set; }
    }
}
