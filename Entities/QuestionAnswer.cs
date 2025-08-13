using Ankets.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ankets.Entities
{
    public class QuestionAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerId { get; set; }

        public int ResponseId { get; set; }
        [ForeignKey("ResponseId")]
        public SurveyResponse SurveyResponse { get; set; }

        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        public string? AnswerText { get; set; }

        public int[]? SelectedOptionIds { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? NumericValue { get; set; }

        public DateOnly? DateValue { get; set; }

        public bool? BooleanValue { get; set; }

        public string[]? FileUrls { get; set; }

        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    }
}