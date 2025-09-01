using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class PositionSkill
    {
        [Key]
        public int PositionSkillId { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

        public bool IsRequired { get; set; }
    }
}
