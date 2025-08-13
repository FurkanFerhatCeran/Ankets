using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ankets.Entities
{
    public class QuestionType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TypesId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypesName { get; set; }

        public string? TypesDescription { get; set; }

        [Required]
        [MaxLength(30)]
        public string InputType { get; set; }

        public bool AllowsMultiple { get; set; } = false;

        public bool RequiresOptions { get; set; } = false;

        public int? MaxOptions { get; set; }

        public int? MinOptions { get; set; }

        public bool IsActive { get; set; } = true;
    }
}