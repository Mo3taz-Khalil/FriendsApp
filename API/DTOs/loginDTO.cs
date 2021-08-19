using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class loginDTO
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
         public string  password { get; set; }
    }
}