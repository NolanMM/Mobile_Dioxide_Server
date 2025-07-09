using System.ComponentModel.DataAnnotations;

namespace Mobile_Server_Dioxide.DTOs
{
    public class LoginDto
    {
        [Required]
        public string username { get; set; } = string.Empty;

        [Required]
        public string password { get; set; } = string.Empty;
    }
}
