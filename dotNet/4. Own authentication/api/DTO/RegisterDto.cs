using System.ComponentModel.DataAnnotations;

namespace api.DTO
{
    public class RegisterDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string username { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string password { get; set; }
    }
}