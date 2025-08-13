using Ankets.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ankets.Entities
{
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionId { get; set; }

        public int SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }

        public int QuestionTypeId { get; set; }
        [ForeignKey("QuestionTypeId")]
        public QuestionType QuestionType { get; set; }

        [Required]
        public string QuestionTitle { get; set; }

        public string? QuestionDescription { get; set; }

        public bool IsRequired { get; set; } = false;

        [Column(TypeName = "jsonb")]
        public string? ConditionalLogic { get; set; }

        [Column(TypeName = "jsonb")]
        public string? ValidationRules { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}