using System.ComponentModel.DataAnnotations;

namespace API_Fall_Of_The_Sun.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string? ConfirmationToken { get; set; }

    }
}
