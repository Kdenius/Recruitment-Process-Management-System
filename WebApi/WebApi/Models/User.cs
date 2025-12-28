using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public string? PasswordHash { get; set; }
        public string? VerifyToken { get; set; }
        public bool IsVerified { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
        public String? RefreshToken {  get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation property
        public int RoleId { get; set; }
        public Role Role { get; set; }
        [JsonIgnore]
        public ICollection<Interview>? Interviews { get; set; }

    }
}

