using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class InterviewRating
    {
        [Key]
        public int RatingId { get; set; }

        public int InterviewId { get; set; }
        public Interview Interview { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

        public int Rating { get; set; } // 1 thi 5
        public string Remark {  get; set; }
    }
}
