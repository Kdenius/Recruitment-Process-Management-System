using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class CandidateSkill
    {
        [Key]
        public int CandidateSkillId { get; set; }

        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

        public int YearsOfExperience { get; set; }
        public bool IsVerified { get; set; }
    }
}
