using System.ComponentModel.DataAnnotations;

namespace Mobile_Server_Dioxide.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        public string username { get; set; } = string.Empty;

        [Required]
        public string password { get; set; } = string.Empty;

        [Required]
        public string email { get; set; } = string.Empty;

        [Required]
        public string first_name { get; set; } = string.Empty;

        [Required]
        public string last_name { get; set; } = string.Empty;
    }
}
