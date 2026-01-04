using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
        public string Location { get; set; } // Remote ke offline 
        public string Type { get; set; } //ful time, part time, contract
        public int BaseSalary { get; set; }
        public int MaxSalary { get; set; }

        public string? CloserReason { get; set; }

        public int Rounds { get; set; }

        public DateTime CreatedAt { get; set; }

        public int RecruiterId { get; set; }
        public User Recruiter { get; set; }

        [JsonIgnore]
        public ICollection<PositionSkill> PositionSkills { get; set; }
        [JsonIgnore]
        public ICollection<CandidateApplication> CandidateApplications { get; set; }
    }
}
