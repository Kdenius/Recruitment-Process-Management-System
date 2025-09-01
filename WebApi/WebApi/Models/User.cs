using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public ICollection<Interview> Interviews { get; set; }

    }
}

