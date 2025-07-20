using System.ComponentModel.DataAnnotations;

namespace Mobile_Server_Dioxide.DTOs
{
    public class UpdateUsernameDto
    {
        [Required]
        [MaxLength(150)]
        public string Username { get; set; } = string.Empty;
    }
}
