using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.Models
{
    public class Position
    {
        [Key]
        public int PositionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } // Open,On Hold, Closed

        public string CloserReason { get; set; }

        public int Rounds { get; set; }

        public DateTime CreatedAt { get; set; }

        public int RecruiterId { get; set; }
        public User Recruiter { get; set; }

        public ICollection<PositionSkill> PositionSkills { get; set; }
        public ICollection<CandidateApplication> CandidateApplications { get; set; }
    }
}
