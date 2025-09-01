using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.Models
{
    public class Candidate
    {
        [Key]
        public int CandidateId { get; set; }

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

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(30)]
        public string CurrentStatus { get; set; } 

        public DateTime? JoiningDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? ResumeUrl { get; set; }

        public ICollection<CandidateApplication> CandidateApplications { get; set; }
        public ICollection<CandidateSkill> CandidateSkills { get; set; }
    }
}

