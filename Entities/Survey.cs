using Ankets.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ankets.Entities
{
    public class Survey
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SurveyId { get; set; }

        [Required]
        [MaxLength(200)]
        public string SurveyTitle { get; set; }

        public string? SurveyDescription { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public SurveyCategory? SurveyCategory { get; set; }

        public int CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public User Creator { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsAnonymous { get; set; } = true;

        public bool AllowMultipleResponses { get; set; } = false;

        public bool RequiresLogin { get; set; } = false;

        public bool IsPublic { get; set; } = true;

        [MaxLength(7)]
        public string ThemeColor { get; set; } = "#007bff";

        [MaxLength(255)]
        public string? LogoUrl { get; set; }

        public int ViewCount { get; set; } = 0;

        public int ResponseCount { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal CompletionRate { get; set; } = 0;

        public int AverageTimeMinutes { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PublishedAt { get; set; }
    }
}