using Ankets.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ankets.Entities
{
    public class SurveyShare
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShareId { get; set; }

        public int SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }

        [Required]
        [MaxLength(100)]
        public string ShareToken { get; set; }

        public int SharedBy { get; set; }
        [ForeignKey("SharedBy")]
        public User SharedByUser { get; set; }

        public bool IsActive { get; set; } = true;

        public int ClickCount { get; set; } = 0;

        public DateTime? ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}