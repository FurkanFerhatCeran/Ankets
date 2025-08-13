using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ankets.Entities
{
    public class SurveyCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoriesId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoriesName { get; set; }

        public string? CategoriesDescription { get; set; }

        [MaxLength(7)]
        public string? Color { get; set; }
    }
}