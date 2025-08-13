using Ankets.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ankets.Entities
{
    public class SurveyAnalytic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnalyticId { get; set; }

        public int SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }

        [Required]
        public DateOnly AnalyticDate { get; set; }

        public int TotalViews { get; set; } = 0;

        public int TotalStarts { get; set; } = 0;

        public int TotalCompletions { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal CompletionRate { get; set; } = 0;

        public int AverageTimeMinutes { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal BounceRate { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}