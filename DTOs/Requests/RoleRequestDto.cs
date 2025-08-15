using System.ComponentModel.DataAnnotations;

namespace Ankets.DTOs.Requests
{
    public class RoleRequestDto
    {
        [Required(ErrorMessage = "Rol ad� gereklidir")]
        [MaxLength(50)]
        public string RoleName { get; set; }
    }
}