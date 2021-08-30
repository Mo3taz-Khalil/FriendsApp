using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class registerDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string password { get; set; }
    }
}