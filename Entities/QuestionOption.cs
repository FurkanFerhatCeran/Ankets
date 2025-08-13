using Ankets.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ankets.Entities
{
    public class QuestionOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OptionId { get; set; }

        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        [Required]
        public string OptionText { get; set; }

        [MaxLength(100)]
        public string? OptionValue { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsOtherOption { get; set; } = false;

        [Column(TypeName = "jsonb")]
        public string? ConditionalLogic { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}