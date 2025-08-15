using System.ComponentModel.DataAnnotations;

namespace Ankets.DTOs.Requests
{
    public class RoleRequestDto
    {
        [Required(ErrorMessage = "Rol adý gereklidir")]
        [MaxLength(50)]
        public string RoleName { get; set; }
    }
}