using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobile_Server_Dioxide.Entities
{
    [Table("auth_user", Schema = "dbo")]
    public class User_DBO
    {
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(128)]
        public string password { get; set; } = string.Empty;

        public DateTimeOffset? last_login { get; set; }

        [Required]
        public bool is_superuser { get; set; }

        [Required]
        [MaxLength(150)]
        public string username { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string first_name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string last_name { get; set; } = string.Empty;

        [Required]
        [MaxLength(254)]
        public string email { get; set; } = string.Empty;

        [Required]
        public bool is_staff { get; set; }

        [Required]
        public bool is_active { get; set; }

        [Required]
        public DateTimeOffset date_joined { get; set; }
    }
}
