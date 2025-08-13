using Ankets.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ankets.Entities
{
    public class AiAnalysis
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AiAnalysisId { get; set; }

        public int SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]
        public string AiResponse { get; set; }

        public string? Summary { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? ConfidenceLevel { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}