using Ankets.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Ankets.Entities
{
    public class SurveyResponse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResponseId { get; set; }

        public int SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }

        public int? RespondentId { get; set; }
        [ForeignKey("RespondentId")]
        public User? Respondent { get; set; }

        [Required]
        [MaxLength(255)]
        public string SessionId { get; set; }

        public IPAddress? IpAddress { get; set; }

        public bool IsCompleted { get; set; } = false;

        public bool IsPartial { get; set; } = false;

        [Column(TypeName = "decimal(5,2)")]
        public decimal CompletionPercentage { get; set; } = 0;

        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        public DateTime? EndTime { get; set; }

        public int? DurationMinutes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}