using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Skill
    {
        [Key]
        public int SkillId { get; set; }

        [Required]
        [StringLength(50)]
        public string SkillName { get; set; }

        public ICollection<PositionSkill> PositionSkills { get; set; }
        public ICollection<CandidateSkill> CandidateSkills { get; set; }
        public ICollection<InterviewRating> InterviewRatings { get; set; }
    }
}
