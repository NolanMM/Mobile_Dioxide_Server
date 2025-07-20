using System.ComponentModel.DataAnnotations;

namespace Mobile_Server_Dioxide.DTOs
{
    public class UpdateNameDto
    {
        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; } = string.Empty;
    }
}
